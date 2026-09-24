namespace WasionOne.API.Models;

// Registro por evento/revisión de disponibilidad de un servidor. Cada
// checada de monitoreo es su propia fila (no un acumulado diario) — el
// usuario confirmó que estos 3 módulos (Servidores, Red, Almacenamiento)
// van como pantallas separadas, no consolidadas. Los servidores existen
// en cualquiera de las plantas, por eso sí lleva Planta (a diferencia de
// lo que se había asumido inicialmente).
public class DisponibilidadServidor
{
    public int Id { get; set; }

    // "ID" del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones (opcional: no todos los
    // orígenes traen un ID propio).
    public string? IdOrigen { get; set; }

    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow.Date;
    public TimeSpan Hora { get; set; }

    public string Servidor { get; set; } = string.Empty;
    public string? Ip { get; set; }
    public string? Servicio { get; set; }

    // Valores esperados: "Disponible", "Intermitente", "Caído" (texto
    // libre a propósito, igual que en Tickets, para no romper una
    // importación por un valor fuera de lista).
    public string Estado { get; set; } = "Disponible";

    public int? TiempoRespuestaMs { get; set; }
    public int? TiempoCaidaMin { get; set; }
    public decimal DisponibilidadPorcentaje { get; set; } = 100;

    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
