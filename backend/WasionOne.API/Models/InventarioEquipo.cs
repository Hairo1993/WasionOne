namespace WasionOne.API.Models;

// Elemento del inventario electrónico de IT, ligado al nodo operativo
// AreaUbicacion (el Área IT en una Ubicación específica).
public class InventarioEquipo
{
    public int Id { get; set; }

    // "Planta": coincide con el catálogo de Ubicaciones (igual que en
    // Tickets).
    public int AreaUbicacionId { get; set; }
    public AreaUbicacion? AreaUbicacion { get; set; }

    // Código/etiqueta interna de inventario (ej. "IT-0001"). Único.
    public string Codigo { get; set; } = string.Empty;

    // Detalle de ubicación física más fino que la Planta (AreaUbicacion).
    public string? Almacen { get; set; }
    public string? UbicacionExacta { get; set; }

    // Ej. "Laptop", "Desktop", "Monitor", "Switch", "Router", "Impresora", "Otro".
    public string TipoEquipo { get; set; } = string.Empty;

    public string? Hostname { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Serial { get; set; } = string.Empty;

    public string? Ram { get; set; }
    public string? Procesador { get; set; }
    public string? Almacenamiento { get; set; }
    public string? SistemaOperativo { get; set; }
    public string? MacWireless { get; set; }
    public string? MacEthernet { get; set; }

    public string? UsuarioAsignado { get; set; }

    // Valores esperados: "Activo", "EnReparacion", "Baja", "Resguardo".
    public string Estado { get; set; } = "Activo";

    public DateTime? FechaCompra { get; set; }
    public DateTime? TerminoGarantia { get; set; }

    public DateTime FechaAlta { get; set; } = DateTime.UtcNow;
    public DateTime? FechaBaja { get; set; }

    public string? Observaciones { get; set; }
}
