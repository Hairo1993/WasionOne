namespace WasionOne.API.DTOs;

public class DopingDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }
    public int AreaUbicacionId { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Turno { get; set; }
    public string? NoNomina { get; set; }
    public string? Nombre { get; set; }
    public string? Area { get; set; }
    public string Resultado { get; set; } = string.Empty;
    public DateTime? FechaImportacion { get; set; }
}

public class DopingCrearDto
{
    public int AreaUbicacionId { get; set; }
    public DateTime? Fecha { get; set; }
    public string? Turno { get; set; }
    public string? NoNomina { get; set; }
    public string? Nombre { get; set; }
    public string? Area { get; set; }
    public string Resultado { get; set; } = "Negativo";
}

public class DopingActualizarDto
{
    public string Resultado { get; set; } = string.Empty;
}

public class DopingImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
