namespace WasionOne.API.DTOs;

public class PlaticaDto
{
    public int Id { get; set; }
    public string? IdOrigen { get; set; }

    // null = toda la empresa.
    public int? AreaUbicacionId { get; set; }

    public DateTime FechaEnvio { get; set; }
    public string TemaPolitica { get; set; } = string.Empty;
    public string? ResponsableEnvio { get; set; }
    public string? MedioDifusion { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class PlaticaCrearDto
{
    public int? AreaUbicacionId { get; set; }
    public DateTime FechaEnvio { get; set; }
    public string TemaPolitica { get; set; } = string.Empty;
    public string? ResponsableEnvio { get; set; }
    public string? MedioDifusion { get; set; }
}

public class PlaticaActualizarDto
{
    public int? AreaUbicacionId { get; set; }
    public DateTime FechaEnvio { get; set; }
    public string TemaPolitica { get; set; } = string.Empty;
    public string? ResponsableEnvio { get; set; }
    public string? MedioDifusion { get; set; }
}

public class PlaticaImportarResultadoDto
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
