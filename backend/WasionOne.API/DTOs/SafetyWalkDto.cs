namespace WasionOne.API.DTOs;

public class SafetyWalkDto
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string? Area { get; set; }
    public int AreaUbicacionId { get; set; }
    public decimal Cumplimiento { get; set; }
    public bool AsistioGerenteCalidad { get; set; }
    public bool AsistioGerenteProduccion { get; set; }
    public bool AsistioGerenteLogistica { get; set; }
    public bool AsistioGerenteSoporteTecnico { get; set; }
    public bool AsistioGerenteProyectos { get; set; }
    public bool AsistioGerenteCompras { get; set; }
    public bool AsistioGerenteRh { get; set; }
    public bool AsistioCoordinadorCsh { get; set; }
    public bool AsistioSecretario { get; set; }
    public bool AsistioVocal1 { get; set; }
    public bool AsistioVocal2 { get; set; }
    public bool AsistioVocal3 { get; set; }
    public bool AsistioVocal4 { get; set; }
    public bool AsistioVocal5 { get; set; }
    public DateTime? FechaImportacion { get; set; }
}

public class SafetyWalkCrearDto
{
    public DateTime Fecha { get; set; }
    public string? Area { get; set; }
    public int AreaUbicacionId { get; set; }
    public decimal Cumplimiento { get; set; }
    public bool AsistioGerenteCalidad { get; set; }
    public bool AsistioGerenteProduccion { get; set; }
    public bool AsistioGerenteLogistica { get; set; }
    public bool AsistioGerenteSoporteTecnico { get; set; }
    public bool AsistioGerenteProyectos { get; set; }
    public bool AsistioGerenteCompras { get; set; }
    public bool AsistioGerenteRh { get; set; }
    public bool AsistioCoordinadorCsh { get; set; }
    public bool AsistioSecretario { get; set; }
    public bool AsistioVocal1 { get; set; }
    public bool AsistioVocal2 { get; set; }
    public bool AsistioVocal3 { get; set; }
    public bool AsistioVocal4 { get; set; }
    public bool AsistioVocal5 { get; set; }
}

// No incluye Fecha ni AreaUbicacionId — juntas son la llave del registro y
// no se editan después de creado (evita chocar con otro registro existente).
public class SafetyWalkActualizarDto
{
    public string? Area { get; set; }
    public decimal Cumplimiento { get; set; }
    public bool AsistioGerenteCalidad { get; set; }
    public bool AsistioGerenteProduccion { get; set; }
    public bool AsistioGerenteLogistica { get; set; }
    public bool AsistioGerenteSoporteTecnico { get; set; }
    public bool AsistioGerenteProyectos { get; set; }
    public bool AsistioGerenteCompras { get; set; }
    public bool AsistioGerenteRh { get; set; }
    public bool AsistioCoordinadorCsh { get; set; }
    public bool AsistioSecretario { get; set; }
    public bool AsistioVocal1 { get; set; }
    public bool AsistioVocal2 { get; set; }
    public bool AsistioVocal3 { get; set; }
    public bool AsistioVocal4 { get; set; }
    public bool AsistioVocal5 { get; set; }
}

public class SafetyWalkImportarResultadoDto : IResultadoImportacion
{
    public int TotalFilas { get; set; }
    public int Creados { get; set; }
    public int Actualizados { get; set; }
    public int Omitidos { get; set; }
    public List<string> Errores { get; set; } = new();
}
