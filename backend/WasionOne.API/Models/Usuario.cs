namespace WasionOne.API.Models;

public class Usuario
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;

    // Uno de los valores de Roles (CEO, Director, ResponsableDepartamento, ResponsableArea).
    public string Rol { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    // Posición en la jerarquía Dirección -> Departamento -> Área. Según el
    // Rol, solo uno de estos tres debería estar asignado (un CEO no necesita
    // ninguno; un Director solo DireccionId; etc.).
    public int? DireccionId { get; set; }
    public Direccion? Direccion { get; set; }

    public int? DepartamentoId { get; set; }
    public Departamento? Departamento { get; set; }

    public int? AreaId { get; set; }
    public Area? Area { get; set; }

    // Módulos de captura a los que este usuario tiene acceso (selección
    // manual del administrador, independiente de su nodo en la jerarquía).
    public ICollection<UsuarioModulo> ModulosAsignados { get; set; } = new List<UsuarioModulo>();

    // Nivel de captura por Planta dentro de cada módulo asignado (20/sep/2026).
    // Sin filas aquí para un módulo = ese módulo se captura en todas las
    // Plantas del Área (comportamiento histórico). Ver UsuarioModuloUbicacion.
    public ICollection<UsuarioModuloUbicacion> ModuloUbicacionesAsignadas { get; set; } = new List<UsuarioModuloUbicacion>();
}
