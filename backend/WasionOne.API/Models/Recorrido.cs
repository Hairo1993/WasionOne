namespace WasionOne.API.Models;

// Registro de un recorrido/rondín de vigilancia. Un registro por
// recorrido realizado (no un agregado por día), igual que Incidentes o
// Auditorías de IT.
public class Recorrido
{
    public int Id { get; set; }

    // "ID" del sistema de origen, si el registro fue importado. Llave
    // para no duplicar en reimportaciones.
    public string? IdOrigen { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public DateTime? Fecha { get; set; }

    public string? Operador { get; set; }

    // "Área / Tipo" del recorrido — texto libre (no hay catálogo para esto).
    public string? AreaTipo { get; set; }

    public string Estado { get; set; } = "Completado";

    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraFin { get; set; }

    // Duración en minutos. Si no viene explícita (captura manual o Excel),
    // se calcula a partir de HoraInicio/HoraFin.
    public int? DuracionMinutos { get; set; }

    public string? Hallazgos { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
