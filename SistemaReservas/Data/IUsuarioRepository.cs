using SistemaReservas.Models;

namespace SistemaReservas.Data;

public interface IUsuarioRepository
{
    Task<Usuario?> ValidarUsuarioAsync(string username, string password);
}
