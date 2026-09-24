namespace WasionOne.API.Models;

// PROPUESTO — pendiente de confirmar con el usuario: el equipo de
// producto todavía no dio la lista exacta de campos para "Brigadas". Este
// diseño (un registro por evento/integrante de brigada, llave de negocio
// Folio) es razonable dado el patrón ya establecido en el sistema, pero
// debe revisarse con el usuario antes de considerarse definitivo.
//
// Registro único por Folio — se edita conforme cambian sus datos (estado,
// última capacitación), no es un histórico de versiones. Mismo patrón que
// Mantenimiento Vehicular (llave de negocio única, aquí el Folio).
public class Brigada
{
    public int Id { get; set; }

    // PROPUESTO: llave de negocio única. Un registro por Folio, que se
    // actualiza en vez de duplicarse.
    public string Folio { get; set; } = string.Empty;

    // PROPUESTO.
    public DateTime Fecha { get; set; }

    // "Planta": ubicación física del catálogo. Obligatoria.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // PROPUESTO — catálogo fijo: "Evacuación", "Primeros Auxilios",
    // "Combate de Incendios", "Búsqueda y Rescate".
    public string TipoBrigada { get; set; } = string.Empty;

    // PROPUESTO — texto libre.
    public string NombreBrigadista { get; set; } = string.Empty;

    // PROPUESTO — catálogo fijo: "Jefe de Brigada", "Brigadista".
    public string PuestoBrigada { get; set; } = string.Empty;

    // PROPUESTO — catálogo fijo: "Activo", "Inactivo". Default "Activo".
    public string Estado { get; set; } = "Activo";

    // PROPUESTO.
    public DateTime? FechaUltimaCapacitacion { get; set; }

    // PROPUESTO — texto libre.
    public string? Observaciones { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
