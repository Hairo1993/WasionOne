namespace WasionOne.API.Models;

// Bitácora de actualizaciones aplicadas a equipos críticos de IT
// (firmware, sistema operativo, etc.). El "Código" identifica al equipo,
// no al registro: un mismo equipo tiene varias actualizaciones a lo
// largo del tiempo (distintos meses), así que la llave de unicidad es la
// combinación Código + Fecha de registro (un mismo equipo no puede tener
// dos registros de actualización con la misma fecha), no el Código por
// sí solo.
public class ActualizacionEquipoCritico
{
    public int Id { get; set; }

    // "Planta": ubicación física del catálogo. Obligatoria (igual que
    // Disponibilidad de servidores/Red y % de Almacenamiento).
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    public string Tipo { get; set; } = string.Empty;

    // Identificador del equipo crítico (texto libre — no se liga al
    // catálogo de Inventario electrónico, mismo criterio que "Código de
    // activo" en Auditorías).
    public string Codigo { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;

    public bool TeniaActualizacion { get; set; }
    public bool SeAplico { get; set; }

    public string? Observaciones { get; set; }
    public string? Responsable { get; set; }

    public DateTime FechaRegistro { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
