namespace WasionOne.API.DTOs;

public class AuditoriaEquipoDto
{
    public int Id { get; set; }
    public string? Folio { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime? FechaProgramada { get; set; }
    public DateTime? FechaRealizada { get; set; }
    public string? Area { get; set; }
    public string? Almacen { get; set; }
    public string? CodigoActivo { get; set; }
    public string? DescripcionActivo { get; set; }
    public string? Responsable { get; set; }
    public string? Tipo { get; set; }
    public int? Revisados { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaImportacion { get; set; }
}

public class AuditoriaEquipoCrearDto
{
    public int AreaUbicacionId { get; set; }
    public DateTime? FechaProgramada { get; set; }
    public DateTime? FechaRealizada { get; set; }
    public string? Area { get; set; }
    public string? Almacen { get; set; }
    public string? CodigoActivo { get; set; }
    public string? DescripcionActivo { get; set; }
    public string? Responsable { get; set; }
    public string? Tipo { get; set; }
    public int? Revisados { get; set; }
}

public class AuditoriaEquipoActualizarDto
{
    public DateTime? FechaRealizada { get; set; }
    public string? Responsable { get; set; }
    public int? Revisados { get; set; }
    public string Estado { get; set; } = string.Empty;
}

public class AuditoriaEquipoImportarResultadoDto
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
