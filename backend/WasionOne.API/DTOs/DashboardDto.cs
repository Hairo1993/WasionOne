namespace WasionOne.API.DTOs;

// DTOs del Dashboard de Dirección / Dashboard de Departamento (22/sep/2026).
// No son una tabla propia: es una vista agregada de solo lectura armada en
// memoria (DashboardService) a partir de los 24 módulos de captura que ya
// existen, organizada según la jerarquía Dirección -> Departamento -> Área.

public class DashboardModuloDto
{
    public string Clave { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    // Total de registros del módulo. Es un COUNT en casi todos los casos;
    // la única excepción es Alcoholimetría, que es SUM(Cantidad) — cada
    // fila ahí ya es un agregado por Planta+Fecha+Turno, no una aplicación
    // individual (el usuario confirmó que quiere "número de aplicaciones").
    public decimal Total { get; set; }

    // Null si el módulo no tiene un KPI definido (ej. Recorridos, Lockers,
    // Pláticas, Dopings, Reuniones con proveedor, Disponibilidad y
    // Abastecimiento) o si no hay datos suficientes para calcularlo.
    public decimal? Kpi { get; set; }

    // Etiqueta legible del KPI (ej. "% Resueltos/Cerrados", "Días sin
    // incidentes"). Va siempre junto con Kpi: null si Kpi es null.
    public string? KpiEtiqueta { get; set; }
}

public class DashboardAreaDto
{
    public int AreaId { get; set; }
    public string Nombre { get; set; } = string.Empty;

    // false para Áreas que todavía no tienen ningún módulo de captura
    // construido (ej. RH, Proyectos, Seguridad e Higiene) — se muestran
    // igual en el dashboard, pero con "Sin datos capturados todavía" en
    // vez de una lista de módulos vacía sin explicación.
    public bool TieneModulosCaptura { get; set; }

    public List<DashboardModuloDto> Modulos { get; set; } = new();
}

public class DashboardDepartamentoDto
{
    public int DepartamentoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public List<DashboardAreaDto> Areas { get; set; } = new();
}

public class DashboardDireccionDto
{
    public int DireccionId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public List<DashboardDepartamentoDto> Departamentos { get; set; } = new();
}
