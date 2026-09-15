namespace WasionOne.API.DTOs;

public class RespaldoDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime FechaRespaldo { get; set; }
    public string SistemaAplicacion { get; set; } = string.Empty;
    public string? SoftwareUtilizado { get; set; }
    public string? TipoRespaldo { get; set; }
    public string? Responsable { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class RespaldoCrearDto
{
    public int AreaUbicacionId { get; set; }
    public DateTime FechaRespaldo { get; set; }
    public string SistemaAplicacion { get; set; } = string.Empty;
    public string? SoftwareUtilizado { get; set; }
    public string? TipoRespaldo { get; set; }
    public string? Responsable { get; set; }
    public string? Observaciones { get; set; }
}

public class RespaldoActualizarDto
{
    public string Estado { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}

public class RespaldoImportarResultadoDto
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
