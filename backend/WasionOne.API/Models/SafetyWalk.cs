namespace WasionOne.API.Models;

// Registro de una caminata de seguridad (Safety Walk) realizada en una
// Fecha/Planta (Área "Seguridad e Higiene", Departamento "Seguridad"). No
// trae folio: la llave de negocio es la combinación (Fecha, AreaUbicacionId)
// — un registro por Fecha exacta + Planta, se actualiza en vez de
// duplicarse.
public class SafetyWalk
{
    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    // Zona/área operativa mencionada en el reporte (texto libre). OJO: no
    // tiene relación con la jerarquía Dirección/Departamento/Área del
    // sistema — no confundir con AreaUbicacionId.
    public string? Area { get; set; }

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Porcentaje de cumplimiento (0-100) capturado directamente por el
    // usuario — no se calcula de otros campos.
    public decimal Cumplimiento { get; set; }

    // Asistencia del Comité de Seguridad e Higiene (13 columnas booleanas).
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

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
