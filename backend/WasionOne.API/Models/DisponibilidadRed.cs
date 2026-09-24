namespace WasionOne.API.Models;

// Registro por evento/revisión de disponibilidad de un dispositivo de red
// (switch, router, access point, etc.), por Planta. Mismo criterio que
// DisponibilidadServidor: una fila por checada, no un acumulado diario.
public class DisponibilidadRed
{
    public int Id { get; set; }

    public string? IdOrigen { get; set; }

    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow.Date;
    public TimeSpan Hora { get; set; }

    public string Dispositivo { get; set; } = string.Empty;
    public string? Ip { get; set; }
    public string? TipoDispositivo { get; set; }

    // Valores esperados: "Disponible", "Intermitente", "Caído".
    public string Estado { get; set; } = "Disponible";

    public decimal? LatenciaMs { get; set; }
    public decimal? PerdidaPaquetesPorcentaje { get; set; }
    public int? TiempoCaidaMin { get; set; }
    public decimal DisponibilidadPorcentaje { get; set; } = 100;

    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }

    public DateTime? FechaImportacion { get; set; }
}
