namespace WasionOne.API.DTOs;

public class LoginDto
{
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRespuestaDto
{
    public string Token { get; set; } = string.Empty;
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = new List<string>();

    // Posición en la jerarquía y módulos habilitados: se mandan también en
    // el cuerpo de la respuesta (no solo como claims del JWT) para que el
    // frontend arme el menú y aplique sus guards sin tener que decodificar
    // el token.
    public int? DireccionId { get; set; }
    public int? DepartamentoId { get; set; }
    public int? AreaId { get; set; }
    public IEnumerable<string> Modulos { get; set; } = new List<string>();

    // Nivel de captura por Planta dentro de cada módulo (20/sep/2026). Un
    // módulo de la lista de arriba que no aparece aquí = todas las Plantas
    // del Área (sin restricción). Se manda también en el cuerpo de la
    // respuesta (no solo como claims del JWT) para que el frontend filtre
    // el selector de Planta de cada pantalla sin decodificar el token.
    public IEnumerable<ModuloUbicacionAsignadaDto> ModuloPlantas { get; set; } = new List<ModuloUbicacionAsignadaDto>();
}
