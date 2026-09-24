namespace WasionOne.API.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public int? DireccionId { get; set; }
    public int? DepartamentoId { get; set; }
    public int? AreaId { get; set; }

    // Claves de Modulos a las que este usuario tiene acceso.
    public List<string> Modulos { get; set; } = new();

    // Nivel de captura por Planta dentro de cada módulo (20/sep/2026). Un
    // módulo de la lista de arriba que no aparece aquí = todas las Plantas
    // del Área (sin restricción).
    public List<ModuloUbicacionAsignadaDto> ModuloPlantas { get; set; } = new();
}

public class UsuarioCrearDto
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public int? DireccionId { get; set; }
    public int? DepartamentoId { get; set; }
    public int? AreaId { get; set; }
    public List<string> Modulos { get; set; } = new();
    public List<ModuloUbicacionAsignadaDto> ModuloPlantas { get; set; } = new();
}

public class UsuarioActualizarDto
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public int? DireccionId { get; set; }
    public int? DepartamentoId { get; set; }
    public int? AreaId { get; set; }
    public List<string> Modulos { get; set; } = new();
    public List<ModuloUbicacionAsignadaDto> ModuloPlantas { get; set; } = new();
}

public class UsuarioCambiarPasswordDto
{
    public string NuevaPassword { get; set; } = string.Empty;
}

public class ModuloDto
{
    public string Clave { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;

    // A qué Área pertenece (IT = 3, Seguridad Patrimonial = 1) — para que
    // la pantalla de Usuarios sepa qué lista de Plantas ofrecer al
    // configurar el nivel de captura por Planta de este módulo.
    public int AreaId { get; set; }
}

// Una Planta permitida para un Usuario dentro de un módulo específico
// (20/sep/2026). Lista plana, un renglón por combinación Módulo+Planta —
// refleja 1:1 la tabla UsuarioModuloUbicacion. Un módulo asignado (ver
// UsuarioDto.Modulos) que NO aparece aquí en absoluto significa "todas
// las Plantas del Área" (sin restricción, comportamiento histórico).
public class ModuloUbicacionAsignadaDto
{
    public string ModuloClave { get; set; } = string.Empty;
    public int AreaUbicacionId { get; set; }
}
