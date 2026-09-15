namespace WasionOne.API.DTOs;

public class IncidenteCriticoDto
{
    public int Id { get; set; }
    public string? IdFallaOrigen { get; set; }
    public int AreaUbicacionId { get; set; }

    public DateTime Fecha { get; set; }
    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraFin { get; set; }
    public decimal? DuracionHoras { get; set; }

    // Null = impacta a todos los departamentos.
    public int? DepartamentoId { get; set; }

    public string? Area { get; set; }
    public string? Linea { get; set; }
    public string? Severidad { get; set; }
    public string? Tipo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Responsable { get; set; }
    public string? Causa { get; set; }
    public string? Detalles { get; set; }
    public string? Contramedida { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaImportacion { get; set; }
}

public class IncidenteCriticoCrearDto
{
    public int AreaUbicacionId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan? HoraInicio { get; set; }
    public TimeSpan? HoraFin { get; set; }
    public decimal? DuracionHoras { get; set; }

    // Null = impacta a todos los departamentos.
    public int? DepartamentoId { get; set; }

    public string? Area { get; set; }
    public string? Linea { get; set; }
    public string? Severidad { get; set; }
    public string? Tipo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Responsable { get; set; }
    public string? Causa { get; set; }
    public string? Detalles { get; set; }
    public string? Contramedida { get; set; }
}

public class IncidenteCriticoActualizarDto
{
    public string Estado { get; set; } = string.Empty;
    public string? Causa { get; set; }
    public string? Detalles { get; set; }
    public string? Contramedida { get; set; }
    public TimeSpan? HoraFin { get; set; }
    public decimal? DuracionHoras { get; set; }
}

public class IncidenteCriticoImportarResultadoDto
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
