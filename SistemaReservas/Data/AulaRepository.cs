using System.Data;
using Microsoft.Data.SqlClient;
using SistemaReservas.Models;

namespace SistemaReservas.Data;

public class AulaRepository : IAulaRepository
{
    public Task<DataTable> ObtenerAulasDataTableAsync() => Task.Run(ObtenerAulasDataTable);
    public Task<List<Aula>> ObtenerAulasObjetosAsync() => Task.Run(ObtenerAulasObjetos);

    private const string SelectAulas = "SELECT AulaId, Nombre, Capacidad FROM Aulas ORDER BY Nombre";

    // Escenario desconectado.
    public DataTable ObtenerAulasDataTable()
    {
        var table = new DataTable();
        using var connection = new SqlConnection(Conexion.CadenaConexion);
        using var adapter = new SqlDataAdapter(SelectAulas, connection);
        adapter.Fill(table);
        return table;
    }

    // Escenario conectado.
    public List<Aula> ObtenerAulasObjetos()
    {
        var aulas = new List<Aula>();
        using var connection = new SqlConnection(Conexion.CadenaConexion);
        connection.Open();
        using var command = new SqlCommand(SelectAulas, connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            aulas.Add(new Aula
            {
                AulaId = Convert.ToInt32(reader["AulaId"]),
                Nombre = reader["Nombre"].ToString() ?? "",
                Capacidad = Convert.ToInt32(reader["Capacidad"])
            });
        }
        return aulas;
    }
}
