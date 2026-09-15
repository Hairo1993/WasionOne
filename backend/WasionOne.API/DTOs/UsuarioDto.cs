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
}