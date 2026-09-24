namespace WasionOne.API.Models;

// Registro de una observación de seguridad reportada (Área "Seguridad e
// Higiene", Departamento "Seguridad"). Un registro por evento — Folio es
// la llave de negocio única, se actualiza en vez de duplicarse (mismo
// patrón que MantenimientoVehicular.Vin).
public class ObservacionSeguridad
{
    public int Id { get; set; }

    // Llave de negocio: folio único del reporte.
    public string Folio { get; set; } = string.Empty;

    public DateTime Fecha { get; set; }

    // Quien levantó la observación (texto libre).
    public string Usuario { get; set; } = string.Empty;

    // "N. Nómina" de quien levantó la observación. Opcional, texto libre.
    public string? NNomina { get; set; }

    public string PersonaObservada { get; set; } = string.Empty;

    public string Empresa { get; set; } = string.Empty;

    // Zona/área operativa mencionada en el reporte (texto libre). OJO: no
    // tiene relación con la jerarquía Dirección/Departamento/Área del
    // sistema — no confundir con AreaUbicacionId.
    public string? Area { get; set; }

    // Catálogo fijo (no es enum ni tabla, solo string libre validado en el
    // frontend): "Acto inseguro", "Condición insegura", "Reconocimiento
    // positivo".
    public string Tipo { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    // Catálogo fijo: "EPP", "Orden y limpieza", "Manejo de materiales
    // peligrosos", "Ergonomía", "Maquinaria y equipo", "Procedimientos de
    // trabajo", "Instalaciones eléctricas", "Trabajo en alturas", "Otro".
    public string Categorias { get; set; } = string.Empty;

    // Catálogo fijo: "Abierto", "En proceso", "Cerrado".
    public string Estado { get; set; } = "Abierto";

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
