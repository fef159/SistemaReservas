using System.Data;
using Microsoft.Data.SqlClient;
using SistemaReservas.Models;

namespace SistemaReservas.Data;

public class ReservaRepository : IReservaRepository
{
    public Task<DataTable> ObtenerReservasDataTableAsync() => Task.Run(ObtenerReservasDataTable);
    public Task<List<Reserva>> ObtenerReservasObjetosAsync() => Task.Run(ObtenerReservasObjetos);

    // Escenario desconectado: nombres legibles para la agenda.
    public DataTable ObtenerReservasDataTable()
    {
        var table = new DataTable();
        using var connection = new SqlConnection(Conexion.CadenaConexion);
        const string sql = """
            SELECT R.ReservaId, U.NombreCompleto AS Usuario, A.Nombre AS Aula,
                   R.Fecha, R.Hora, R.Motivo
            FROM Reservas R
            INNER JOIN Usuarios U ON R.UsuarioId = U.UsuarioId
            INNER JOIN Aulas A ON R.AulaId = A.AulaId
            ORDER BY R.Fecha DESC, R.Hora DESC, R.ReservaId DESC
            """;
        using var adapter = new SqlDataAdapter(sql, connection);
        adapter.Fill(table);
        return table;
    }

    // Escenario conectado.
    public List<Reserva> ObtenerReservasObjetos()
    {
        var reservas = new List<Reserva>();
        using var connection = new SqlConnection(Conexion.CadenaConexion);
        connection.Open();
        const string sql = """
            SELECT ReservaId, AulaId, UsuarioId, Fecha, Hora, Motivo
            FROM Reservas ORDER BY Fecha DESC, Hora DESC, ReservaId DESC
            """;
        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            reservas.Add(new Reserva
            {
                ReservaId = Convert.ToInt32(reader["ReservaId"]),
                AulaId = Convert.ToInt32(reader["AulaId"]),
                UsuarioId = Convert.ToInt32(reader["UsuarioId"]),
                Fecha = Convert.ToDateTime(reader["Fecha"]),
                Hora = (TimeSpan)reader["Hora"],
                Motivo = reader["Motivo"].ToString() ?? ""
            });
        }
        return reservas;
    }
}
