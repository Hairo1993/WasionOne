namespace WasionOne.API.DTOs;

public class ReunionProveedorDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }
    public DateTime? Fecha { get; set; }
    public TimeSpan? Hora { get; set; }
    public string? Proveedor { get; set; }
    public int AreaUbicacionId { get; set; }
    public string? AsuntoMotivo { get; set; }
    public string? Asistentes { get; set; }
    public string? MinutaAcuerdos { get; set; }
    public string? RegistradoPor { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class ReunionProveedorCrearDto
{
    public int AreaUbicacionId { get; set; }
    public DateTime? Fecha { get; set; }
    public TimeSpan? Hora { get; set; }
    public string? Proveedor { get; set; }
    public string? AsuntoMotivo { get; set; }
    public string? Asistentes { get; set; }
    public string? MinutaAcuerdos { get; set; }
    public string? RegistradoPor { get; set; }
}

public class ReunionProveedorActualizarDto
{
    public string? Asistentes { get; set; }
    public string? MinutaAcuerdos { get; set; }
}

public class ReunionProveedorImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
