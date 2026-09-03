using System.Data;
using SistemaReservas.Models;

namespace SistemaReservas.Data;

public interface IReservaRepository
{
    Task<DataTable> ObtenerReservasDataTableAsync();
    Task<List<Reserva>> ObtenerReservasObjetosAsync();
}
