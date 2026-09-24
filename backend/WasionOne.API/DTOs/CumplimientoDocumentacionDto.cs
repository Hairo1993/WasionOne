namespace WasionOne.API.DTOs;

public class CumplimientoDocumentacionDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Proveedor { get; set; }
    public string? TipoContrato { get; set; }
    public string? Area { get; set; }
    public string? Responsable { get; set; }
    public decimal? MontoIvaIncluido { get; set; }
    public string? Moneda { get; set; }
    public bool FirmaDireccion { get; set; }
    public bool FirmaLegal { get; set; }
    public bool FirmaFinanzas { get; set; }
    public bool AprobadoLeninLi { get; set; }
    public bool Firmado { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string? Renovacion { get; set; }
    public string? Estatus { get; set; }
    public string? Carpeta { get; set; }
    public int AreaUbicacionId { get; set; }

    // Calculado: (FechaVencimiento.Date - Hoy).Days. Nunca se persiste ni
    // se acepta del cliente/Excel — cambia todos los días, se calcula en
    // cada lectura.
    public int DiasPorVencer { get; set; }

    public DateTime? FechaImportacion { get; set; }
}

public class CumplimientoDocumentacionCrearDto
{
    public string Code { get; set; } = string.Empty;
    public string? Proveedor { get; set; }
    public string? TipoContrato { get; set; }
    public string? Area { get; set; }
    public string? Responsable { get; set; }
    public decimal? MontoIvaIncluido { get; set; }
    public string? Moneda { get; set; }
    public bool FirmaDireccion { get; set; }
    public bool FirmaLegal { get; set; }
    public bool FirmaFinanzas { get; set; }
    public bool AprobadoLeninLi { get; set; }
    public bool Firmado { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string? Renovacion { get; set; }
    public string? Estatus { get; set; }
    public string? Carpeta { get; set; }
    public int AreaUbicacionId { get; set; }
}

// No incluye Code — es la llave del registro y no se edita después de creado.
public class CumplimientoDocumentacionActualizarDto
{
    public string? Proveedor { get; set; }
    public string? TipoContrato { get; set; }
    public string? Area { get; set; }
    public string? Responsable { get; set; }
    public decimal? MontoIvaIncluido { get; set; }
    public string? Moneda { get; set; }
    public bool FirmaDireccion { get; set; }
    public bool FirmaLegal { get; set; }
    public bool FirmaFinanzas { get; set; }
    public bool AprobadoLeninLi { get; set; }
    public bool Firmado { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public string? Renovacion { get; set; }
    public string? Estatus { get; set; }
    public string? Carpeta { get; set; }
    public int AreaUbicacionId { get; set; }
}

public class CumplimientoDocumentacionImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
