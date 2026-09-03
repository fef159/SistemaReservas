using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using SistemaReservas.Data;
using SistemaReservas.Models;
using SistemaReservas.MVVM;
using SistemaReservas.ViewModels;
using SistemaReservas.Views;

internal static class Program
{
    private static int checks;
    private static string output = "";

    [STAThread]
    private static void Main(string[] args)
    {
        var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
        foreach (string file in new[] { "Styles", "ViewTemplates" })
            app.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri($"pack://application:,,,/SistemaReservas;component/Themes/{file}.xaml")
            });
        output = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "previews"));
        Directory.CreateDirectory(output);
        var bindingLog = new StringWriter();
        PresentationTraceSources.DataBindingSource.Listeners.Add(new TextWriterTraceListener(bindingLog));
        PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;

        SearchChecks();
        ArchitectureChecks();
        ViewModelChecks();
        AuthenticationChecks();
        VisualChecks(new FakeAulas(), new FakeReservas(), "sample-");
        if (args.Contains("--database")) DatabaseChecks();
        Check(string.IsNullOrEmpty(bindingLog.ToString()), "Sin errores de binding: " + bindingLog);
        Console.WriteLine($"PASS: {checks} comprobaciones. Previews: {output}");
        app.Shutdown();
    }

    private static void SearchChecks()
    {
        var table = SampleData.Aulas();
        foreach (string query in new[] { "'", "[", "]", "%", "*", "O'Brien", "100%", "[A]" })
        {
            TableSearch.Apply(table, query, "Nombre");
            Check(table.DefaultView.Count == 1, "Búsqueda literal: " + query);
        }
        TableSearch.Apply(table, "AULA", "Nombre");
        Check(table.DefaultView.Count == 1, "Búsqueda sin distinguir mayúsculas");
        TableSearch.Apply(table, "  ", "Nombre");
        Check(table.DefaultView.Count == 2, "Limpiar filtro");
        TableSearch.Apply(table, "sin coincidencias", "Nombre");
        Check(table.DefaultView.Count == 0, "Búsqueda sin coincidencias");
    }

    private static void ArchitectureChecks()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "SistemaReservas", "SistemaReservas.csproj")))
            directory = directory.Parent;
        if (directory == null) throw new InvalidOperationException("No se encontró el proyecto para verificar su arquitectura.");
        string root = Path.Combine(directory.FullName, "SistemaReservas");
        foreach (string file in Directory.GetFiles(Path.Combine(root, "Views"), "*.xaml.cs"))
        {
            string source = File.ReadAllText(file);
            Check(!source.Contains("Repository") && !source.Contains("ViewModel") &&
                  !source.Contains("DataContext") && !source.Contains("async ") &&
                  !source.Contains("Task") && !source.Contains("_Click"),
                  "Vista sin lógica ni creación de dependencias: " + Path.GetFileName(file));
        }
        foreach (string file in Directory.GetFiles(Path.Combine(root, "ViewModels"), "*.cs"))
        {
            string source = File.ReadAllText(file);
            Check(!source.Contains("SistemaReservas.Views") && !source.Contains("System.Windows.Controls") &&
                  !source.Contains("Application.Current") && !source.Contains("new Sql") &&
                  !source.Contains("new AulaRepository") && !source.Contains("new ReservaRepository"),
                  "ViewModel independiente de vistas y SQL: " + Path.GetFileName(file));
        }
    }

    private static void ViewModelChecks()
    {
        var repository = new FakeAulas();
        var vm = new AulaViewModel(repository);
        var notifications = new List<string?>();
        vm.PropertyChanged += (_, e) => notifications.Add(e.PropertyName);
        var pending = new TaskCompletionSource<DataTable>();
        repository.Load = () => pending.Task;
        Task first = vm.ActualizarCommand.ExecuteAsync();
        Task duplicate = vm.ActualizarCommand.ExecuteAsync();
        Check(!vm.ActualizarCommand.CanExecute(null) && repository.TableCalls == 1, "Consulta simultánea bloqueada");
        pending.SetResult(repository.Table.Copy());
        Wait(Task.WhenAll(first, duplicate));
        Check(vm.ActualizarCommand.CanExecute(null) && vm.AulasDataTable.Rows.Count == 2, "Comando habilitado después de cargar");
        Check(notifications.Contains(nameof(vm.AulasDataTable)) && notifications.Contains(nameof(vm.Resumen)), "Notificación de datos y contador");

        repository.Load = null;
        vm.Busqueda = "O'Brien";
        Check(vm.AulasDataTable.DefaultView.Count == 1 && !vm.MostrarMensaje, "Filtro en ViewModel");
        Wait(vm.ActualizarCommand.ExecuteAsync());
        Check(vm.AulasDataTable.DefaultView.Count == 1, "Filtro conservado al actualizar");
        vm.Busqueda = "sin coincidencias";
        Check(vm.MostrarMensaje && vm.Resumen == "0 de 2 aulas", "Estado sin resultados en ViewModel");

        repository.Load = () => Task.FromException<DataTable>(new InvalidOperationException("simulated"));
        Wait(vm.ActualizarCommand.ExecuteAsync());
        Check(vm.AulasDataTable.Rows.Count == 2 && vm.Estado.StartsWith("No se pudo"), "Fallo de actualización conserva datos previos");
        Check(vm.ActualizarCommand.CanExecute(null), "Reintento habilitado tras fallo");
        var failed = new AulaViewModel(repository);
        Wait(failed.ActualizarCommand.ExecuteAsync());
        Check(failed.Resumen == "Sin conexión" && failed.MostrarMensaje, "Error inicial visible");

        repository.Load = null;
        repository.Table.Clear();
        Wait(failed.ActualizarCommand.ExecuteAsync());
        Check(failed.MostrarMensaje && failed.MensajeVacio.Contains("Todavía"), "Colección vacía legítima");
        repository.Table = SampleData.Aulas();
        Wait(failed.ActualizarCommand.ExecuteAsync());
        Check(!failed.MostrarMensaje && failed.Resumen == "2 de 2 aulas", "Recuperación después de fallo");

        var reservas = new ReservaViewModel(new FakeReservas());
        Wait(reservas.ActualizarCommand.ExecuteAsync());
        Check((DateTime)reservas.ReservasDataTable.DefaultView[0]["Fecha"] == SampleData.Today.Date.AddDays(1), "Orden cronológico de reservas");
        foreach (string query in new[] { "Responsable dos", "Taller", "O'Brien" })
        {
            reservas.Busqueda = query;
            Check(reservas.ReservasDataTable.DefaultView.Count == 1, "Filtro de reservas: " + query);
        }

        var aulasObjetos = new AulaViewModel(new FakeAulas(), false);
        var reservasObjetos = new ReservaViewModel(new FakeReservas(), false);
        Wait(Task.WhenAll(aulasObjetos.ActualizarCommand.ExecuteAsync(), reservasObjetos.ActualizarCommand.ExecuteAsync()));
        Check(aulasObjetos.Aulas.Count == 2 && aulasObjetos.AulasDataTable.Rows.Count == 0, "Aulas: modo de objetos");
        Check(reservasObjetos.Reservas.Count == 3 && reservasObjetos.ReservasDataTable.Rows.Count == 0, "Reservas: modo de objetos");

        Pagina? destino = null;
        var inicio = new InicioViewModel(new FakeAulas(), new FakeReservas(), page => destino = page, () => SampleData.Today);
        var culture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("ar-SA");
            Wait(inicio.ActualizarCommand.ExecuteAsync());
            Check(inicio.ReservasHoy == "2" && inicio.TotalAulas == "2" && inicio.CapacidadTotal == "70", "Indicadores calculados con fecha independiente de cultura");
            Check((TimeSpan)inicio.AgendaHoy![0]["Hora"] == TimeSpan.FromHours(8), "Agenda de hoy ordenada por hora");
        }
        finally { CultureInfo.CurrentCulture = culture; }
        inicio.VerReservasCommand.Execute(null);
        Check(destino == Pagina.Reservas, "Acceso rápido mediante comando");

        var shell = new ShellViewModel(new FakeAulas(), new FakeReservas());
        Check(shell.Contenido is InicioViewModel && shell.EsInicio, "Navegación inicial");
        shell.NavegarCommand.Execute(Pagina.Aulas);
        Check(shell.Contenido is AulaViewModel && shell.EsAulas && !shell.EsInicio, "Navegación y selección en ViewModel");
        shell.NavegarCommand.Execute("invalid");
        Check(shell.Contenido is AulaViewModel, "Destino inválido rechazado");
        shell.NavegarCommand.Execute(Pagina.NuevaReserva);
        ((NuevaReservaViewModel)shell.Contenido).VerReservasCommand.Execute(null);
        Check(shell.Contenido is ReservaViewModel && shell.TituloPagina == "Reservas", "Navegación desde nueva reserva");
        var windows = new FakeWindows();
        new MainViewModel(windows).AbrirLoginCommand.Execute(null);
        Check(windows.LoginCalls == 1, "Apertura de login delegada al servicio");
    }

    private static void AuthenticationChecks()
    {
        var users = new FakeUsuarios();
        var vm = new LoginViewModel(users);
        Wait(vm.IngresarCommand.ExecuteAsync());
        Check(users.Calls == 0 && vm.Error.Contains("Completa"), "Campos vacíos no consultan el repositorio");
        vm.Usuario = "  incorrecto  ";
        vm.Contrasena = "test-only";
        Wait(vm.IngresarCommand.ExecuteAsync());
        Check(users.LastUsername == "incorrecto" && vm.Error.Contains("no son correctos"), "Credenciales inválidas y normalización de usuario");
        Check(vm.Contrasena == "", "Contraseña liberada después de autenticar");

        int success = 0;
        vm.Autenticado += () => success++;
        vm.Usuario = "  demo  ";
        vm.Contrasena = "test-only";
        Wait(vm.IngresarCommand.ExecuteAsync());
        Wait(vm.IngresarCommand.ExecuteAsync());
        Check(success == 1 && !vm.IngresarCommand.CanExecute(null), "Autenticación válida notifica una sola vez");

        var response = new TaskCompletionSource<Usuario?>();
        users.Login = () => response.Task;
        var closed = new LoginViewModel(users) { Usuario = "demo", Contrasena = "test-only" };
        closed.Autenticado += () => success++;
        Task request = closed.IngresarCommand.ExecuteAsync();
        Check(closed.TextoIngreso == "Ingresando…" && !closed.IngresarCommand.CanExecute(null), "Estado de autenticación en curso");
        closed.Desactivar();
        response.SetResult(new Usuario { UsuarioId = 1 });
        Wait(request);
        Check(success == 1 && closed.Contrasena == "", "Cerrar login impide una apertura tardía");

        users.Login = () => Task.FromException<Usuario?>(new InvalidOperationException("simulated"));
        var failure = new LoginViewModel(users) { Usuario = "demo", Contrasena = "test-only" };
        Wait(failure.IngresarCommand.ExecuteAsync());
        Check(failure.Error.Contains("No se pudo conectar") && failure.IngresarCommand.CanExecute(null), "Error de login recuperable");

        var loginVm = new LoginViewModel(new FakeUsuarios());
        var login = new LoginView { DataContext = loginVm };
        Render((FrameworkElement)login.Content, "login", 490, 651);
        var username = (TextBox)login.FindName("txtUsuario");
        var password = (PasswordBox)login.FindName("txtPassword");
        username.Text = "demo";
        password.Password = "test-only";
        Flush();
        Check(loginVm.Usuario == "demo" && loginVm.Contrasena == "test-only", "Controles de acceso actualizan bindings");
        var button = (Button)login.FindName("IngresarButton");
        Check(ReferenceEquals(button.Command, loginVm.IngresarCommand), "Botón de acceso enlazado al comando");
        Wait(loginVm.IngresarCommand.ExecuteAsync());
        Flush();
        Check(password.Password == "", "Limpieza de contraseña reflejada en PasswordBox");
        login.Close();
    }

    private static void VisualChecks(IAulaRepository aulas, IReservaRepository reservas, string prefix)
    {
        var welcome = new MainWindow { DataContext = new MainViewModel(new FakeWindows()) };
        Render((FrameworkElement)welcome.Content, "welcome", 1040, 641);
        welcome.Close();
        var shellVm = new ShellViewModel(aulas, reservas);
        var shell = new ShellView { DataContext = shellVm };
        var home = Materialize<InicioView>(shell);
        Load(home);
        Check(((TextBlock)home.FindName("StatusLabel")).Text.StartsWith("Información actualizada"), prefix + "Inicio enlazado");
        Render(shell, prefix + "dashboard", 1200, 761);
        Render(shell, prefix + "dashboard-compact", 1000, 641);

        shellVm.NavegarCommand.Execute(Pagina.Aulas);
        var aulasView = Materialize<AulasView>(shell);
        Load(aulasView);
        var aulasVm = (AulaViewModel)aulasView.DataContext;
        var grid = (DataGrid)aulasView.FindName("dgAulas");
        Check(grid.Items.Count == aulasVm.AulasDataTable.Rows.Count, prefix + "Directorio enlazado");
        Render(shell, prefix + "aulas", 1200, 761);
        var search = (TextBox)aulasView.FindName("txtBuscar");
        search.Text = "__sin_coincidencias__'[%*]";
        Flush();
        Check(aulasVm.Busqueda == search.Text && grid.Items.Count == 0, prefix + "Búsqueda por binding");
        Check(((TextBlock)aulasView.FindName("EmptyLabel")).Visibility == Visibility.Visible, prefix + "Estado vacío enlazado");
        Render(shell, prefix + "aulas-empty", 1000, 641);
        search.Text = "";
        Flush();
        Check(grid.Items.Count == aulasVm.AulasDataTable.Rows.Count, prefix + "Limpiar búsqueda restaura filas");
        Check(Equals(((Button)shell.FindName("AulasButton")).Tag, true), prefix + "Selección de navegación enlazada");

        shellVm.NavegarCommand.Execute(Pagina.Reservas);
        var reservasView = Materialize<ReservasView>(shell);
        Load(reservasView);
        Render(shell, prefix + "reservas", 1200, 761);
        var agenda = (DataGrid)reservasView.FindName("ReservationsGrid");
        if (agenda.Items.Count > 0)
        {
            var first = (DataRowView)agenda.Items[0];
            var timeCell = (TextBlock)agenda.Columns[1].GetCellContent(first);
            Check(timeCell.Text == ((TimeSpan)first["Hora"]).ToString(@"hh\:mm"), prefix + "Hora visible en formato HH:mm");
        }

        shellVm.NavegarCommand.Execute(Pagina.NuevaReserva);
        Materialize<NuevaReservaView>(shell);
        Render(shell, prefix + "new-reservation", 1200, 761);

        foreach (UserControl legacy in new UserControl[]
        {
            new AulasDataTableView { DataContext = new AulaViewModel(aulas) },
            new AulasObjetosView { DataContext = new AulaViewModel(aulas, false) },
            new ReservasDataTableView { DataContext = new ReservaViewModel(reservas) },
            new ReservasObjetosView { DataContext = new ReservaViewModel(reservas, false) }
        })
        {
            Load(legacy);
            Render(legacy, prefix + legacy.GetType().Name, 1000, 640);
            Check(!((LoadableViewModel)legacy.DataContext).Estado.StartsWith("No se pudo"), prefix + legacy.GetType().Name);
        }
    }

    private static void DatabaseChecks()
    {
        var aulas = new AulaRepository();
        var reservas = new ReservaRepository();
        Check(aulas.ObtenerAulasDataTable().Rows.Count == aulas.ObtenerAulasObjetos().Count, "SQL: modos de aulas consistentes");
        Check(reservas.ObtenerReservasDataTable().Rows.Count == reservas.ObtenerReservasObjetos().Count, "SQL: modos de reservas consistentes");
        var invalid = new LoginViewModel(new UsuarioRepository())
        {
            Usuario = "__verification_" + Guid.NewGuid().ToString("N"), Contrasena = "invalid"
        };
        Wait(invalid.IngresarCommand.ExecuteAsync());
        Check(invalid.Error.Contains("no son correctos"), "SQL: credenciales inexistentes rechazadas");
        VisualChecks(aulas, reservas, "");

        string original = Conexion.CadenaConexion;
        try
        {
            Conexion.CadenaConexion = "Server=tcp:127.0.0.1,1;Database=ReservasDB;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=1;ConnectRetryCount=0;Pooling=False";
            var vm = new AulaViewModel(aulas);
            Wait(vm.ActualizarCommand.ExecuteAsync());
            Check(vm.Resumen == "Sin conexión", "SQL: conexión fallida controlada");
            Render(new AulasView { DataContext = vm }, "connection-error", 850, 620);
            Conexion.CadenaConexion = original;
            Wait(vm.ActualizarCommand.ExecuteAsync());
            Check(vm.AulasDataTable.Rows.Count > 0 && vm.Resumen != "Sin conexión", "SQL: recuperación al reintentar");
        }
        finally { Conexion.CadenaConexion = original; }
    }

    private static T Materialize<T>(FrameworkElement root) where T : FrameworkElement
    {
        Layout(root, 1200, 761);
        return FindVisual<T>(root) ?? throw new InvalidOperationException("DataTemplate no creó " + typeof(T).Name);
    }

    private static T? FindVisual<T>(DependencyObject parent) where T : FrameworkElement
    {
        if (parent is T match) return match;
        for (int index = 0; index < VisualTreeHelper.GetChildrenCount(parent); index++)
            if (FindVisual<T>(VisualTreeHelper.GetChild(parent, index)) is T child) return child;
        return null;
    }

    private static void Load(FrameworkElement view)
    {
        view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        var vm = (LoadableViewModel)view.DataContext;
        PumpUntil(() => !vm.ActualizarCommand.IsExecuting);
        Flush();
    }

    private static void Wait(Task task)
    {
        PumpUntil(() => task.IsCompleted);
        task.GetAwaiter().GetResult();
    }

    private static void PumpUntil(Func<bool> condition)
    {
        var watch = Stopwatch.StartNew();
        while (!condition())
        {
            if (watch.Elapsed > TimeSpan.FromSeconds(20)) throw new TimeoutException("La consulta no terminó.");
            var frame = new DispatcherFrame();
            var timer = new DispatcherTimer(TimeSpan.FromMilliseconds(20), DispatcherPriority.Background, (_, _) => frame.Continue = false, Dispatcher.CurrentDispatcher);
            Dispatcher.PushFrame(frame);
            timer.Stop();
        }
    }

    private static void Flush() => Dispatcher.CurrentDispatcher.Invoke(() => { }, DispatcherPriority.ContextIdle);

    private static void Layout(FrameworkElement element, int width, int height)
    {
        element.Measure(new Size(width, height));
        element.Arrange(new Rect(0, 0, width, height));
        element.UpdateLayout();
        Flush();
    }

    private static void Render(FrameworkElement element, string name, int width, int height)
    {
        Layout(element, width, height);
        var bitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        var background = new DrawingVisual();
        using (var drawing = background.RenderOpen())
            drawing.DrawRectangle((Brush)Application.Current.Resources["Canvas"], null, new Rect(0, 0, width, height));
        bitmap.Render(background);
        bitmap.Render(element);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var stream = File.Create(Path.Combine(output, name + ".png"));
        encoder.Save(stream);
    }

    private static void Check(bool success, string message)
    {
        if (!success) throw new InvalidOperationException("FAIL: " + message);
        checks++;
        Console.WriteLine("PASS: " + message);
    }
}
