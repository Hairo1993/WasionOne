namespace WasionOne.API.DTOs;

// PROPUESTO — pendiente de confirmar con el usuario (ver comentario en
// Models/Brigada.cs).
public class BrigadaDto
{
    public int Id { get; set; }
    public string Folio { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public int AreaUbicacionId { get; set; }
    public string TipoBrigada { get; set; } = string.Empty;
    public string NombreBrigadista { get; set; } = string.Empty;
    public string PuestoBrigada { get; set; } = string.Empty;
    public string Estado { get; set; } = "Activo";
    public DateTime? FechaUltimaCapacitacion { get; set; }
    public string? Observaciones { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class BrigadaCrearDto
{
    public string Folio { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public int AreaUbicacionId { get; set; }
    public string TipoBrigada { get; set; } = string.Empty;
    public string NombreBrigadista { get; set; } = string.Empty;
    public string PuestoBrigada { get; set; } = string.Empty;
    public string Estado { get; set; } = "Activo";
    public DateTime? FechaUltimaCapacitacion { get; set; }
    public string? Observaciones { get; set; }
}

// No incluye Folio — es la llave del registro y no se edita después de
// creado (mismo criterio que MantenimientoVehicularActualizarDto con Vin).
public class BrigadaActualizarDto
{
    public DateTime Fecha { get; set; }
    public int AreaUbicacionId { get; set; }
    public string TipoBrigada { get; set; } = string.Empty;
    public string NombreBrigadista { get; set; } = string.Empty;
    public string PuestoBrigada { get; set; } = string.Empty;
    public string Estado { get; set; } = "Activo";
    public DateTime? FechaUltimaCapacitacion { get; set; }
    public string? Observaciones { get; set; }
}

public class BrigadaImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
