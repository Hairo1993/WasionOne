namespace WasionOne.API.Models;

// Bitácora histórica de Cumplimiento de EPP (Equipo de Protección Personal)
// — Seguridad e Higiene. El "IdEpp" identifica AL EQUIPO DE PROTECCIÓN, no
// a la fila: un mismo EPP tiene varios registros a través del tiempo
// (existencias/solicitud/abastecimiento cambian mes a mes), así que la
// llave de unicidad es la combinación IdEpp + Fecha de registro (mismo
// criterio que Código + Fecha de registro en Actualizaciones de equipos
// críticos de IT), no el IdEpp por sí solo.
public class CumplimientoEpp
{
    public int Id { get; set; }

    // Identificador del EPP dado por el usuario (texto libre — no se liga a
    // ningún catálogo de Inventario).
    public string IdEpp { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    // Texto libre (ej. "Pieza"/"Par").
    public string? Unidad { get; set; }

    // Texto libre (ej. "Chica"/"38").
    public string? Talla { get; set; }

    public int? VidaUtilCantidad { get; set; }

    // Texto libre (ej. "Meses"/"Años").
    public string? VidaUtilUnidad { get; set; }

    public int? Minimo { get; set; }
    public int? Maximo { get; set; }
    public int? Existencias { get; set; }

    // Cantidad solicitada.
    public int? Solicitud { get; set; }

    // Cantidad abastecida/surtida.
    public int? AbastecimientoStock { get; set; }

    // "Planta": ubicación física del catálogo. Obligatoria (igual que
    // Actualizaciones de equipos críticos).
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Fecha del registro/versión de este EPP. Campo agregado (no venía en
    // la lista original del usuario) porque una bitácora histórica necesita
    // fecha para poder tener varias versiones del mismo IdEpp a través del
    // tiempo — es la segunda mitad de la llave de unicidad junto con IdEpp.
    public DateTime FechaRegistro { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
