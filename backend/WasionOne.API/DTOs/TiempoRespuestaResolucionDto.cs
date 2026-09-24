namespace WasionOne.API.DTOs;

public class TiempoRespuestaResolucionDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public DateTime FechaRecepcion { get; set; }
    public TimeSpan? HoraRecepcion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public TimeSpan? HoraCierre { get; set; }
    public string? Solicitante { get; set; }
    public string? ResponsableCompras { get; set; }
    public string? Tipo { get; set; }
    public string? Prioridad { get; set; }
    public string? Estatus { get; set; }
    public string? Solicitud { get; set; }
    public decimal? SlaCierreHoras { get; set; }
    public string? Observaciones { get; set; }
    public int AreaUbicacionId { get; set; }

    // Persistidos: se calculan una sola vez al capturarse el cierre.
    public decimal? TiempoAtencionHoras { get; set; }
    public decimal? TiempoAtencionDias { get; set; }
    public bool? CumplimientoSla { get; set; }

    // NO persistido: solo tiene sentido mientras el ticket sigue abierto y
    // cambia a cada minuto — se calcula en vivo en cada lectura, contra
    // DateTime.Now, y solo si FechaCierre es null (si ya cerró, este campo
    // regresa null y el dato relevante es TiempoAtencionHoras).
    public decimal? TiempoAbiertoHoras { get; set; }

    public DateTime? FechaImportacion { get; set; }
}

public class TiempoRespuestaResolucionCrearDto
{
    public string Folio { get; set; } = string.Empty;
    public DateTime FechaRecepcion { get; set; }
    public TimeSpan? HoraRecepcion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public TimeSpan? HoraCierre { get; set; }
    public string? Solicitante { get; set; }
    public string? ResponsableCompras { get; set; }
    public string? Tipo { get; set; }
    public string? Prioridad { get; set; }
    public string? Estatus { get; set; }
    public string? Solicitud { get; set; }
    public decimal? SlaCierreHoras { get; set; }
    public string? Observaciones { get; set; }
    public int AreaUbicacionId { get; set; }
}

// No incluye Folio (llave del registro) ni TiempoAtencionHoras/
// TiempoAtencionDias/CumplimientoSla (siempre calculados por el backend,
// nunca aceptados del cliente).
public class TiempoRespuestaResolucionActualizarDto
{
    public DateTime FechaRecepcion { get; set; }
    public TimeSpan? HoraRecepcion { get; set; }
    public DateTime? FechaCierre { get; set; }
    public TimeSpan? HoraCierre { get; set; }
    public string? Solicitante { get; set; }
    public string? ResponsableCompras { get; set; }
    public string? Tipo { get; set; }
    public string? Prioridad { get; set; }
    public string? Estatus { get; set; }
    public string? Solicitud { get; set; }
    public decimal? SlaCierreHoras { get; set; }
    public string? Observaciones { get; set; }
    public int AreaUbicacionId { get; set; }
}

public class TiempoRespuestaResolucionImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
