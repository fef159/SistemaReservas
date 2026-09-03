namespace SistemaReservas.Models;
public class Reserva
{
    public int ReservaId { get; set; }
    public int AulaId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Motivo { get; set; } = "";
}
