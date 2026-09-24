namespace WasionOne.API.Models;

// Registro por ticket de compras/solicitud — un registro por Folio, que se
// actualiza (no se duplica) al reimportar o al capturar su cierre.
public class TiempoRespuestaResolucion
{
    public int Id { get; set; }

    // Llave de negocio: identifica el ticket de forma única. Siempre
    // presente en el sistema de origen (a diferencia del patrón IdOrigen
    // opcional de otros módulos).
    public string Folio { get; set; } = string.Empty;

    public DateTime FechaRecepcion { get; set; }
    public TimeSpan? HoraRecepcion { get; set; }

    // Null mientras el ticket sigue abierto.
    public DateTime? FechaCierre { get; set; }
    public TimeSpan? HoraCierre { get; set; }

    public string? Solicitante { get; set; }
    public string? ResponsableCompras { get; set; }
    public string? Tipo { get; set; }
    public string? Prioridad { get; set; }
    public string? Estatus { get; set; }

    // Detalle/descripción de la solicitud.
    public string? Solicitud { get; set; }

    // SLA objetivo en horas, capturado/importado tal cual (no calculado).
    public decimal? SlaCierreHoras { get; set; }

    public string? Observaciones { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Calculados por el backend, nunca aceptados del cliente ni del Excel.
    // Se calculan y persisten una sola vez, en el momento en que se
    // captura FechaCierre (crear/actualizar/importar) — una vez cerrado el
    // ticket sus insumos ya no cambian. Son horas de RELOJ (calendario),
    // no horas hábiles: implementar un calendario de horas hábiles real
    // requeriría datos de turnos/festivos que no se tienen todavía.
    public decimal? TiempoAtencionHoras { get; set; }
    public decimal? TiempoAtencionDias { get; set; }

    // true/false si se cumplió el SLA (TiempoAtencionHoras <= SlaCierreHoras);
    // null si nunca se dio SlaCierreHoras o el ticket sigue abierto.
    public bool? CumplimientoSla { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
