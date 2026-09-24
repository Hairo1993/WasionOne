namespace WasionOne.API.DTOs;

public class CumplimientoProgramaDto
{
    public int Id { get; set; }
    public int? IdOrigen { get; set; }
    public string? AreaEvaluada { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime FechaReporte { get; set; }
    public string? Hallazgo { get; set; }
    public string? Seguimiento { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string? Estatus { get; set; }
    public string? Prioridad { get; set; }
    public string? Responsable { get; set; }
    public string? Tipo { get; set; }
    public decimal? CumplimientoGeneralPorcentaje { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class CumplimientoProgramaCrearDto
{
    public string? AreaEvaluada { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime FechaReporte { get; set; }
    public string? Hallazgo { get; set; }
    public string? Seguimiento { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string? Estatus { get; set; }
    public string? Prioridad { get; set; }
    public string? Responsable { get; set; }
    public string? Tipo { get; set; }
    public decimal? CumplimientoGeneralPorcentaje { get; set; }
}

// No incluye IdOrigen — es de control/deduplicación de importación, nunca
// editable desde el cliente (mismo criterio que Estacionamiento/
// ReunionProveedor).
public class CumplimientoProgramaActualizarDto
{
    public string? AreaEvaluada { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime FechaReporte { get; set; }
    public string? Hallazgo { get; set; }
    public string? Seguimiento { get; set; }
    public DateTime? FechaCierre { get; set; }
    public string? Estatus { get; set; }
    public string? Prioridad { get; set; }
    public string? Responsable { get; set; }
    public string? Tipo { get; set; }
    public decimal? CumplimientoGeneralPorcentaje { get; set; }
}

public class CumplimientoProgramaImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
