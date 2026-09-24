namespace WasionOne.API.Models;

// Evaluación mensual de proveedores — Seguridad e Higiene. Un registro por
// combinación Proveedor + Planta + Mes evaluado (un proveedor no puede
// tener 2 evaluaciones el mismo mes en la misma Planta) — mismo criterio
// de llave compuesta que Evaluaciones de vigilancia de Seguridad
// Patrimonial.
public class EvaluacionProveedorSegHigiene
{
    public int Id { get; set; }

    public string Proveedor { get; set; } = string.Empty;

    public string Especialidad { get; set; } = string.Empty;

    // "Planta": ubicación física del catálogo.
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Primer día del mes evaluado (ej. 2026-09-01), para poder filtrar por
    // rango de fechas igual que los demás módulos.
    public DateTime Mes { get; set; }

    // Indicador numérico propio de la evaluación, capturado directamente
    // por el usuario — no confundir con el KPI que calcula el Dashboard.
    public decimal? Kpi { get; set; }

    // 0 a 100.
    public decimal Cumplimiento { get; set; }

    // Control de importación. Null si el registro se capturó aquí directamente.
    public DateTime? FechaImportacion { get; set; }
}
