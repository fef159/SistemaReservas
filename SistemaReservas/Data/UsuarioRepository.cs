using Microsoft.Data.SqlClient;
using SistemaReservas.Models;

namespace SistemaReservas.Data;

public class UsuarioRepository : IUsuarioRepository
{
    public Task<Usuario?> ValidarUsuarioAsync(string username, string password) =>
        Task.Run(() => ValidarUsuario(username, password));

    public Usuario? ValidarUsuario(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)) return null;
        using var connection = new SqlConnection(Conexion.CadenaConexion);
        connection.Open();
        // Conserva el esquema de autenticación de la base de datos existente.
        const string sql = """
            SELECT UsuarioId, Username, NombreCompleto
            FROM Usuarios WHERE Username = @username AND Password = @password
            """;
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@username", username.Trim());
        command.Parameters.AddWithValue("@password", password);
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;
        return new Usuario
        {
            UsuarioId = Convert.ToInt32(reader["UsuarioId"]),
            Username = reader["Username"].ToString() ?? "",
            NombreCompleto = reader["NombreCompleto"].ToString() ?? ""
        };
    }
}
