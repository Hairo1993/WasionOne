namespace WasionOne.API.Models;

// Registro de un accidente/incidente de trabajo (Área "Seguridad e
// Higiene", Departamento "Seguridad"). Un registro por evento — Folio es
// la llave de negocio única, se actualiza en vez de duplicarse. El KPI
// "días sin accidentes" se calcula aparte, en otro proceso — este modelo
// es solo el registro individual del accidente.
public class AccidenteTrabajo
{
    public int Id { get; set; }

    // Llave de negocio: folio único del reporte.
    public string Folio { get; set; } = string.Empty;

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Zona/área operativa mencionada en el reporte (texto libre). OJO: no
    // tiene relación con la jerarquía Dirección/Departamento/Área del
    // sistema — no confundir con AreaUbicacionId.
    public string? Area { get; set; }

    public DateTime FechaReporte { get; set; }
    public DateTime FechaOcurrido { get; set; }

    // Persona accidentada.
    public string Nombre { get; set; } = string.Empty;

    public string Compania { get; set; } = string.Empty;

    // Catálogo fijo: "Accidente de trabajo", "Accidente en trayecto",
    // "Incidente/Casi accidente", "Enfermedad de trabajo".
    public string TipoIncidenteAccidente { get; set; } = string.Empty;

    public bool AccidenteConDiasIncapacidad { get; set; }

    // Días de incapacidad. Solo aplica si AccidenteConDiasIncapacidad es
    // true, pero no se valida estrictamente — se captura tal cual.
    public int? Dias { get; set; }

    // Catálogo fijo: "Ninguna", "Corte/Laceración", "Golpe/Contusión",
    // "Fractura", "Quemadura", "Esguince/Torcedura", "Atrapamiento", "Otro".
    public string Lesion { get; set; } = string.Empty;

    // Catálogo fijo: "Cabeza", "Ojos", "Manos", "Dedos", "Brazos",
    // "Piernas", "Pies", "Espalda", "Torso", "Múltiples", "Otro".
    public string ParteLesionada { get; set; } = string.Empty;

    // Catálogo fijo: "Acto inseguro", "Condición insegura", "Falta de
    // EPP", "Falta de capacitación", "Falla de procedimiento", "Falla
    // mecánica/equipo", "Otro".
    public string CausaRaiz { get; set; } = string.Empty;

    // Catálogo fijo: "Abierto", "En proceso", "Cerrado".
    public string EstatusAccion1 { get; set; } = "Abierto";

    // Catálogo fijo: "Abierto", "En proceso", "Cerrado".
    public string EstatusAccion2 { get; set; } = "Abierto";

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
