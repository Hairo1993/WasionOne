namespace WasionOne.API.DTOs;

public class TestConsignaDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }
    public DateTime? Fecha { get; set; }
    public int AreaUbicacionId { get; set; }
    public string ResultadoTest { get; set; } = string.Empty;
    public string? AreaInvolucrada { get; set; }
    public string? ProcedimientoInvolucrado { get; set; }
    public string? Proveedor { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class TestConsignaCrearDto
{
    public int AreaUbicacionId { get; set; }
    public DateTime? Fecha { get; set; }
    public string ResultadoTest { get; set; } = "Aprobado";
    public string? AreaInvolucrada { get; set; }
    public string? ProcedimientoInvolucrado { get; set; }
    public string? Proveedor { get; set; }
}

public class TestConsignaActualizarDto
{
    public string ResultadoTest { get; set; } = string.Empty;
    public string? AreaInvolucrada { get; set; }
    public string? ProcedimientoInvolucrado { get; set; }
    public string? Proveedor { get; set; }
}

public class TestConsignaImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
