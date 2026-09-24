namespace WasionOne.API.DTOs;

public class AccidenteTrabajoDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public int AreaUbicacionId { get; set; }
    public string? Area { get; set; }
    public DateTime FechaReporte { get; set; }
    public DateTime FechaOcurrido { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Compania { get; set; } = string.Empty;
    public string TipoIncidenteAccidente { get; set; } = string.Empty;
    public bool AccidenteConDiasIncapacidad { get; set; }
    public int? Dias { get; set; }
    public string Lesion { get; set; } = string.Empty;
    public string ParteLesionada { get; set; } = string.Empty;
    public string CausaRaiz { get; set; } = string.Empty;
    public string EstatusAccion1 { get; set; } = string.Empty;
    public string EstatusAccion2 { get; set; } = string.Empty;
    public DateTime? FechaImportacion { get; set; }
}

public class AccidenteTrabajoCrearDto
{
    public string Folio { get; set; } = string.Empty;
    public int AreaUbicacionId { get; set; }
    public string? Area { get; set; }
    public DateTime FechaReporte { get; set; }
    public DateTime FechaOcurrido { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Compania { get; set; } = string.Empty;
    public string TipoIncidenteAccidente { get; set; } = string.Empty;
    public bool AccidenteConDiasIncapacidad { get; set; }
    public int? Dias { get; set; }
    public string Lesion { get; set; } = string.Empty;
    public string ParteLesionada { get; set; } = string.Empty;
    public string CausaRaiz { get; set; } = string.Empty;
    public string EstatusAccion1 { get; set; } = "Abierto";
    public string EstatusAccion2 { get; set; } = "Abierto";
}

// No incluye Folio — es la llave del registro y no se edita después de creado.
public class AccidenteTrabajoActualizarDto
{
    public int AreaUbicacionId { get; set; }
    public string? Area { get; set; }
    public DateTime FechaReporte { get; set; }
    public DateTime FechaOcurrido { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Compania { get; set; } = string.Empty;
    public string TipoIncidenteAccidente { get; set; } = string.Empty;
    public bool AccidenteConDiasIncapacidad { get; set; }
    public int? Dias { get; set; }
    public string Lesion { get; set; } = string.Empty;
    public string ParteLesionada { get; set; } = string.Empty;
    public string CausaRaiz { get; set; } = string.Empty;
    public string EstatusAccion1 { get; set; } = string.Empty;
    public string EstatusAccion2 { get; set; } = string.Empty;
}

public class AccidenteTrabajoImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
