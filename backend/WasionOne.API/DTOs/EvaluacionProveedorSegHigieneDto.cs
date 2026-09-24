namespace WasionOne.API.DTOs;

public class EvaluacionProveedorSegHigieneDto
{
    public int Id { get; set; }
    public string Proveedor { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public int AreaUbicacionId { get; set; }
    public DateTime Mes { get; set; }
    public decimal? Kpi { get; set; }
    public decimal Cumplimiento { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class EvaluacionProveedorSegHigieneCrearDto
{
    public string Proveedor { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public int AreaUbicacionId { get; set; }
    public DateTime Mes { get; set; }
    public decimal? Kpi { get; set; }
    public decimal Cumplimiento { get; set; }
}

// No incluye Proveedor/AreaUbicacionId/Mes — son la llave de la evaluación
// y no se editan después de creada (mismo criterio que
// EvaluacionVigilanciaActualizarDto).
public class EvaluacionProveedorSegHigieneActualizarDto
{
    public string Especialidad { get; set; } = string.Empty;
    public decimal? Kpi { get; set; }
    public decimal Cumplimiento { get; set; }
}

public class EvaluacionProveedorSegHigieneImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
