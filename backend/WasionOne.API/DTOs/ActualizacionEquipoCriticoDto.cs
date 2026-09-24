namespace WasionOne.API.DTOs;

public class ActualizacionEquipoCriticoDto
{
    public int Id { get; set; }
    public int AreaUbicacionId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool TeniaActualizacion { get; set; }
    public bool SeAplico { get; set; }
    public string? Observaciones { get; set; }
    public string? Responsable { get; set; }
    public DateTime FechaRegistro { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class ActualizacionEquipoCriticoCrearDto
{
    public int AreaUbicacionId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Codigo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool TeniaActualizacion { get; set; }
    public bool SeAplico { get; set; }
    public string? Observaciones { get; set; }
    public string? Responsable { get; set; }
    public DateTime FechaRegistro { get; set; }
}

// Igual que Alcoholimetria/EvaluacionVigilancia: la llave (Código + Fecha
// de registro) y la Planta no se editan después de creado el registro,
// solo los datos propios de la actualización.
public class ActualizacionEquipoCriticoActualizarDto
{
    public string Tipo { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool TeniaActualizacion { get; set; }
    public bool SeAplico { get; set; }
    public string? Observaciones { get; set; }
    public string? Responsable { get; set; }
}

public class ActualizacionEquipoCriticoImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
