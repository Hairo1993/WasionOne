namespace WasionOne.API.DTOs;

// A diferencia de UsuarioCambiarPasswordDto (que usa un administrador para
// restablecer la contraseña de CUALQUIER usuario sin pedir la anterior),
// este DTO es para que un usuario cambie su PROPIA contraseña desde
// cualquier pantalla — por eso sí exige la contraseña actual.
public class CambiarMiPasswordDto
{
    public string PasswordActual { get; set; } = string.Empty;
    public string PasswordNueva { get; set; } = string.Empty;
}
