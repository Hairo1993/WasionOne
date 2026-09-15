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
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
