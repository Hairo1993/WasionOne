using WasionOne.API.Interfaces;

namespace WasionOne.API.Helpers;

// Helper compartido por los Controllers de captura (IT y Seguridad
// Patrimonial) para aplicar el nivel de captura por Planta que un
// Superadmin puede configurar por usuario+módulo desde /admin/usuarios
// (20/sep/2026). Evita repetir esta lógica en cada uno de los ~20
// Controllers de captura.
//
// De paso, TieneModuloAsignado cierra un hueco que ya existía: antes solo
// el frontend (moduloGuard) ocultaba un módulo si el usuario no lo tenía
// asignado, pero la API en sí no lo verificaba — cualquier usuario
// autenticado podía llamar cualquier endpoint de captura directamente. Al
// construir la restricción por Planta hubo que cerrar ese hueco primero,
// porque si no, "sin restricción" (sin claims "moduloPlanta") sería
// indistinguible de "módulo ni siquiera asignado".
public static class AutorizacionModuloHelper
{
    // ¿El usuario de la petición actual tiene este módulo asignado en
    // absoluto (independientemente de restricción por Planta)?
    public static bool TieneModuloAsignado(IUsuarioContexto contexto, string moduloClave) =>
        contexto.Modulos.Contains(moduloClave);

    // Plantas (AreaUbicacionId) a las que el usuario actual está
    // restringido para este módulo. Null = sin restricción (todas las
    // Plantas del Área).
    public static IReadOnlySet<int>? ObtenerPlantasPermitidas(IUsuarioContexto contexto, string moduloClave)
    {
        var plantas = contexto.ModuloPlantas
            .Where(mp => mp.ModuloClave == moduloClave)
            .Select(mp => mp.AreaUbicacionId)
            .ToHashSet();

        return plantas.Count > 0 ? plantas : null;
    }

    // ¿Puede el usuario capturar/ver un registro con esta Planta (o sin
    // Planta, en los pocos módulos donde es opcional)? Un registro sin
    // Planta ("toda la empresa") no es capturable/visible por un usuario
    // restringido a Plantas específicas.
    public static bool TienePlantaPermitida(IReadOnlySet<int>? plantasPermitidas, int? areaUbicacionId)
    {
        if (plantasPermitidas is null)
        {
            return true;
        }

        return areaUbicacionId.HasValue && plantasPermitidas.Contains(areaUbicacionId.Value);
    }
}
