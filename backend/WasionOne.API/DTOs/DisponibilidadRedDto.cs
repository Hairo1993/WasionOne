namespace WasionOne.API.DTOs;

public class DisponibilidadRedDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Dispositivo { get; set; } = string.Empty;
    public string? Ip { get; set; }
    public string? TipoDispositivo { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal? LatenciaMs { get; set; }
    public decimal? PerdidaPaquetesPorcentaje { get; set; }
    public int? TiempoCaidaMin { get; set; }
    public decimal DisponibilidadPorcentaje { get; set; }
    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class DisponibilidadRedCrearDto
{
    public int AreaUbicacionId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan Hora { get; set; }
    public string Dispositivo { get; set; } = string.Empty;
    public string? Ip { get; set; }
    public string? TipoDispositivo { get; set; }
    public string? Estado { get; set; }
    public decimal? LatenciaMs { get; set; }
    public decimal? PerdidaPaquetesPorcentaje { get; set; }
    public int? TiempoCaidaMin { get; set; }
    public decimal DisponibilidadPorcentaje { get; set; }
    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }
}

public class DisponibilidadRedActualizarDto
{
    public string Estado { get; set; } = string.Empty;
    public decimal? LatenciaMs { get; set; }
    public decimal? PerdidaPaquetesPorcentaje { get; set; }
    public int? TiempoCaidaMin { get; set; }
    public decimal DisponibilidadPorcentaje { get; set; }
    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }
}

public class DisponibilidadRedImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
