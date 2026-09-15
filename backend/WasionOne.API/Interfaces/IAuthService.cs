using WasionOne.API.DTOs;

namespace WasionOne.API.Interfaces;

public interface IAuthService
{
    Task<LoginRespuestaDto?> ValidarCredencialesAsync(LoginDto credenciales);
}