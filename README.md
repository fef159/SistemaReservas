# Sistema de Reservas

Aplicación WPF para .NET 8 con SQL Server. La interfaz comparte colores, tipografía, botones y tablas en `SistemaReservas/Themes/Styles.xaml`.

## Arquitectura MVVM

- **Models:** entidades `Aula`, `Reserva` y `Usuario`.
- **Views:** estructura y presentación XAML. Los archivos `.xaml.cs` solo llaman a `InitializeComponent`; no consultan repositorios, filtran datos ni crean ViewModels.
- **ViewModels:** propiedades observables, filtros, indicadores, mensajes, validación de acceso y comandos. Reciben sus dependencias por constructor y no conocen las ventanas ni los controles.
- **Data:** contratos `IAulaRepository`, `IReservaRepository` e `IUsuarioRepository`, con implementaciones SQL. Se conservan las consultas conectadas y desconectadas; sus métodos asíncronos evitan bloquear la interfaz.
- **Services:** `IWindowService` y `WpfWindowService` gestionan la apertura y el cierre de ventanas. `App.OnStartup` conecta los repositorios, servicios y ViewModels.
- **MVVM y Behaviors:** comandos con `CanExecute`, prevención de ejecuciones simultáneas y adaptadores visuales para `Loaded` y `PasswordBox`.

`ShellViewModel` selecciona el ViewModel de la página. `Themes/ViewTemplates.xaml` decide qué vista mostrar mediante `DataTemplate`; los botones navegan usando comandos. El código de autenticación publica un evento de éxito y el servicio de ventanas realiza la transición al sistema.

Las cuatro vistas alternativas de DataTable y objetos reciben su ViewModel por `DataContext`. Para el modo de objetos se utiliza `new AulaViewModel(repository, false)` o `new ReservaViewModel(repository, false)` desde la composición que hospede la vista.

## Ejecutar

Abre `SistemaReservas.slnx` en Visual Studio o ejecuta:

```powershell
dotnet run --project SistemaReservas/SistemaReservas.csproj
```

Se conserva la conexión existente a `ReservasDB` en la instancia local de SQL Server, con autenticación integrada de Windows. Inicia sesión con las credenciales existentes de la aplicación.

## Cambios

- Pantalla de bienvenida, acceso, navegación lateral y estilos compartidos.
- Inicio con aulas registradas, capacidad total y reservas del día, calculados desde la base de datos.
- Directorio de aulas y agenda con búsqueda, actualización, encabezados legibles y mensajes de carga, error o ausencia de resultados.
- Corrección de búsquedas con comillas, corchetes y comodines; estos caracteres se buscan literalmente.
- Vistas DataTable conectadas a su fuente correcta, conservando también las vistas de objetos.
- Consultas fuera del hilo de interfaz, validación de acceso y control de ventanas para evitar sesiones duplicadas.
- Recursos SQL liberados al finalizar cada consulta. No se cambió el esquema ni se escribieron registros durante la verificación.

La pantalla original `NuevaReservaView` solo contenía un título y no tenía lógica de registro. Ahora informa que el registro está pendiente y ofrece acceso al directorio y a la agenda; no guarda reservas.

## Verificación

El proyecto `Verification` no requiere bibliotecas de pruebas adicionales. Usa repositorios simulados para probar ViewModels sin SQL Server y genera vistas previas usando los controles WPF reales. Verifica filtros, notificaciones, estados vacíos y de error, navegación, autenticación, cierre durante un ingreso pendiente, comandos, bindings y separación entre capas.

```powershell
# Pruebas de arquitectura, ViewModels y controles con repositorios simulados.
dotnet run --project Verification/Verification.csproj

# Añade consultas de solo lectura, vistas de datos y reconexión con ReservasDB.
dotnet run --project Verification/Verification.csproj -- --database
```

La prueba de integración requiere la base existente con registros. La autenticación correcta se prueba con un repositorio simulado; la prueba contra SQL verifica credenciales inexistentes, ya que no se proporcionaron credenciales válidas. Las capturas se guardan bajo `Verification/bin/Debug/net8.0-windows/previews`; las que comienzan por `sample-` utilizan datos simulados.

Si se utiliza una copia con archivos de compilación antiguos, reconstruye antes de ejecutar:

```powershell
dotnet build SistemaReservas/SistemaReservas.csproj -t:Rebuild
```
