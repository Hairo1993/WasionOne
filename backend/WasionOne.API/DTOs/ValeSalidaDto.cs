namespace WasionOne.API.DTOs;

public class ValeSalidaDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }
    public string? Folio { get; set; }
    public string? Solicitante { get; set; }
    public string? Referencia { get; set; }
    public string? ConceptoMotivo { get; set; }
    public string? DetalleMotivo { get; set; }
    public bool ActivoFijo { get; set; }
    public int AreaUbicacionId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaVale { get; set; }
    public DateTime? FechaSalida { get; set; }
    public DateTime? FechaEstimadaRetorno { get; set; }
    public DateTime? FechaRealRetorno { get; set; }
    public string? ArticulosMateriales { get; set; }
    public string? RegistradoPor { get; set; }
    public string? CerradoPor { get; set; }
    public DateTime? FechaRegistro { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class ValeSalidaCrearDto
{
    public int AreaUbicacionId { get; set; }
    public string? Folio { get; set; }
    public string? Solicitante { get; set; }
    public string? Referencia { get; set; }
    public string? ConceptoMotivo { get; set; }
    public string? DetalleMotivo { get; set; }
    public bool ActivoFijo { get; set; }
    public string Estado { get; set; } = "Abierto";
    public DateTime? FechaVale { get; set; }
    public DateTime? FechaSalida { get; set; }
    public DateTime? FechaEstimadaRetorno { get; set; }
    public string? ArticulosMateriales { get; set; }
    public string? RegistradoPor { get; set; }
}

public class ValeSalidaActualizarDto
{
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaEstimadaRetorno { get; set; }
    public DateTime? FechaRealRetorno { get; set; }
    public string? ArticulosMateriales { get; set; }
    public string? CerradoPor { get; set; }
}

public class ValeSalidaImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
