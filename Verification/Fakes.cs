using System.Data;
using SistemaReservas.Data;
using SistemaReservas.Models;
using SistemaReservas.Services;

internal sealed class FakeAulas : IAulaRepository
{
    public DataTable Table { get; set; } = SampleData.Aulas();
    public Func<Task<DataTable>>? Load { get; set; }
    public int TableCalls { get; private set; }
    public int ObjectCalls { get; private set; }

    public Task<DataTable> ObtenerAulasDataTableAsync()
    {
        TableCalls++;
        return Load?.Invoke() ?? Task.FromResult(Table.Copy());
    }

    public Task<List<Aula>> ObtenerAulasObjetosAsync()
    {
        ObjectCalls++;
        return Task.FromResult(Table.AsEnumerable().Select(row => new Aula
        {
            AulaId = row.Field<int>("AulaId"),
            Nombre = row.Field<string>("Nombre")!,
            Capacidad = row.Field<int>("Capacidad")
        }).ToList());
    }
}

internal sealed class FakeReservas : IReservaRepository
{
    public DataTable Table { get; set; } = SampleData.Reservas();
    public int TableCalls { get; private set; }
    public int ObjectCalls { get; private set; }

    public Task<DataTable> ObtenerReservasDataTableAsync()
    {
        TableCalls++;
        return Task.FromResult(Table.Copy());
    }

    public Task<List<Reserva>> ObtenerReservasObjetosAsync()
    {
        ObjectCalls++;
        return Task.FromResult(Table.AsEnumerable().Select(row => new Reserva
        {
            ReservaId = row.Field<int>("ReservaId"),
            Fecha = row.Field<DateTime>("Fecha"),
            Hora = row.Field<TimeSpan>("Hora"),
            Motivo = row.Field<string>("Motivo")!
        }).ToList());
    }
}

internal sealed class FakeUsuarios : IUsuarioRepository
{
    public int Calls { get; private set; }
    public string LastUsername { get; private set; } = "";
    public Func<Task<Usuario?>>? Login { get; set; }
    public Task<Usuario?> ValidarUsuarioAsync(string username, string password)
    {
        Calls++;
        LastUsername = username;
        return Login?.Invoke() ?? Task.FromResult<Usuario?>(
            username == "demo" && password == "test-only" ? new Usuario { UsuarioId = 1, Username = "demo" } : null);
    }
}

internal sealed class FakeWindows : IWindowService
{
    public int LoginCalls { get; private set; }
    public void MostrarLogin() => LoginCalls++;
}

internal static class SampleData
{
    public static readonly DateTime Today = new(2026, 9, 3, 16, 0, 0);

    public static DataTable Aulas()
    {
        var table = new DataTable();
        table.Columns.Add("AulaId", typeof(int));
        table.Columns.Add("Nombre", typeof(string));
        table.Columns.Add("Capacidad", typeof(int));
        table.Rows.Add(1, "Aula 101", 30);
        table.Rows.Add(2, "Sala O'Brien [A] 100% *", 40);
        return table;
    }

    public static DataTable Reservas()
    {
        var table = new DataTable();
        table.Columns.Add("ReservaId", typeof(int));
        table.Columns.Add("Usuario", typeof(string));
        table.Columns.Add("Aula", typeof(string));
        table.Columns.Add("Fecha", typeof(DateTime));
        table.Columns.Add("Hora", typeof(TimeSpan));
        table.Columns.Add("Motivo", typeof(string));
        table.Rows.Add(1, "Responsable uno", "Aula 101", Today.Date, TimeSpan.FromHours(14), "Reunión");
        table.Rows.Add(2, "Responsable dos", "Sala O'Brien [A] 100% *", Today.Date, TimeSpan.FromHours(8), "Taller");
        table.Rows.Add(3, "Responsable tres", "Aula 101", Today.Date.AddDays(1), TimeSpan.FromHours(10), "Exposición");
        return table;
    }
}
