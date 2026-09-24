namespace WasionOne.API.DTOs;

public class EstatusLegalPlantaDto
{
    public int Id { get; set; }
    public int AreaUbicacionId { get; set; }
    public string RequerimientoLegal { get; set; } = string.Empty;
    public string Autoridad { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public DateTime UltimaFechaRealizacion { get; set; }
    public string Estatus { get; set; } = string.Empty;
    public DateTime? FechaImportacion { get; set; }
}

public class EstatusLegalPlantaCrearDto
{
    public int AreaUbicacionId { get; set; }
    public string RequerimientoLegal { get; set; } = string.Empty;
    public string Autoridad { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public DateTime UltimaFechaRealizacion { get; set; }
    public string Estatus { get; set; } = string.Empty;
}

// No incluye AreaUbicacionId/RequerimientoLegal/UltimaFechaRealizacion —
// son la llave del registro y no se editan después de creado, solo
// Autoridad/Frecuencia/Estatus.
public class EstatusLegalPlantaActualizarDto
{
    public string Autoridad { get; set; } = string.Empty;
    public string Frecuencia { get; set; } = string.Empty;
    public string Estatus { get; set; } = string.Empty;
}

public class EstatusLegalPlantaImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
