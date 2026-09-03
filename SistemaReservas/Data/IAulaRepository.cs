using System.Data;
using SistemaReservas.Models;

namespace SistemaReservas.Data;

public interface IAulaRepository
{
    Task<DataTable> ObtenerAulasDataTableAsync();
    Task<List<Aula>> ObtenerAulasObjetosAsync();
}
