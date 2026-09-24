namespace WasionOne.API.DTOs;

public class RecorridoDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Operador { get; set; }
    public string? AreaTipo { get; set; }
    public string Estado { get; set; } = string.Empty;
    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraFin { get; set; }
    public int? DuracionMinutos { get; set; }
    public string? Hallazgos { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class RecorridoCrearDto
{
    public int AreaUbicacionId { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Operador { get; set; }
    public string? AreaTipo { get; set; }
    public string Estado { get; set; } = "Completado";
    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraFin { get; set; }
    public int? DuracionMinutos { get; set; }
    public string? Hallazgos { get; set; }
}

public class RecorridoActualizarDto
{
    public string Estado { get; set; } = string.Empty;
    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraFin { get; set; }
    public int? DuracionMinutos { get; set; }
    public string? Hallazgos { get; set; }
}

public class RecorridoImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
