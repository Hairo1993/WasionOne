namespace WasionOne.API.DTOs;

public class EvaluacionVigilanciaDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Proveedor { get; set; } = string.Empty;
    public int AreaUbicacionId { get; set; }
    public decimal? Cobertura { get; set; }
    public decimal? Expedientes { get; set; }
    public decimal? Uniformidad { get; set; }
    public decimal? Reuniones { get; set; }
    public decimal? Equipamiento { get; set; }
    public decimal? Supervision { get; set; }
    public decimal? CambiosSolicitados { get; set; }
    public decimal? Procedimientos { get; set; }
    public decimal? Capacitacion { get; set; }
    public decimal? Acuerdos { get; set; }
    public decimal Total { get; set; }
    public string? Observaciones { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class EvaluacionVigilanciaCrearDto
{
    public DateTime Fecha { get; set; }
    public string Proveedor { get; set; } = string.Empty;
    public int AreaUbicacionId { get; set; }
    public decimal? Cobertura { get; set; }
    public decimal? Expedientes { get; set; }
    public decimal? Uniformidad { get; set; }
    public decimal? Reuniones { get; set; }
    public decimal? Equipamiento { get; set; }
    public decimal? Supervision { get; set; }
    public decimal? CambiosSolicitados { get; set; }
    public decimal? Procedimientos { get; set; }
    public decimal? Capacitacion { get; set; }
    public decimal? Acuerdos { get; set; }
    public string? Observaciones { get; set; }
}

// No incluye Fecha/Proveedor/AreaUbicacionId — son la llave de la
// evaluación y no se editan después de creada (mismo criterio que
// AlcoholimetriaActualizarDto).
public class EvaluacionVigilanciaActualizarDto
{
    public decimal? Cobertura { get; set; }
    public decimal? Expedientes { get; set; }
    public decimal? Uniformidad { get; set; }
    public decimal? Reuniones { get; set; }
    public decimal? Equipamiento { get; set; }
    public decimal? Supervision { get; set; }
    public decimal? CambiosSolicitados { get; set; }
    public decimal? Procedimientos { get; set; }
    public decimal? Capacitacion { get; set; }
    public decimal? Acuerdos { get; set; }
    public string? Observaciones { get; set; }
}

public class EvaluacionVigilanciaImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
