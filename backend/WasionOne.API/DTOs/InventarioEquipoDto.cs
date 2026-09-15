namespace WasionOne.API.DTOs;

public class InventarioEquipoDto
{
    public int Id { get; set; }
    public int AreaUbicacionId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string? Almacen { get; set; }
    public string? UbicacionExacta { get; set; }
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
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaCompra { get; set; }
    public DateTime? TerminoGarantia { get; set; }
    public DateTime FechaAlta { get; set; }
    public DateTime? FechaBaja { get; set; }
    public string? Observaciones { get; set; }
}

public class InventarioEquipoCrearDto
{
    public int AreaUbicacionId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string? Almacen { get; set; }
    public string? UbicacionExacta { get; set; }
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
    public DateTime? FechaCompra { get; set; }
    public DateTime? TerminoGarantia { get; set; }
    public string? Observaciones { get; set; }
}

public class InventarioEquipoActualizarDto
{
    public string Estado { get; set; } = string.Empty;
    public string? Almacen { get; set; }
    public string? UbicacionExacta { get; set; }
    public string? UsuarioAsignado { get; set; }
    public string? Observaciones { get; set; }
}
