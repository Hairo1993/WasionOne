using System.Security.Cryptography;

namespace WasionOne.API.Helpers;

// Hash de contraseñas con PBKDF2 hecho a mano (sin librerías externas de
// Identity), siguiendo el mismo enfoque que se usa en otros proyectos del
// área. Formato almacenado: "{iteraciones}.{saltBase64}.{hashBase64}".
public static class PasswordHasher
{
    private const int TamanioSalt = 16;
    private const int TamanioHash = 32;
    private const int Iteraciones = 100_000;

    public static string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(TamanioSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteraciones, HashAlgorithmName.SHA256, TamanioHash);

        return $"{Iteraciones}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verificar(string password, string hashAlmacenado)
    {
        var partes = hashAlmacenado.Split('.', 3);
        if (partes.Length != 3 || !int.TryParse(partes[0], out var iteraciones))
        {
            return false;
        }

        byte[] salt;
        byte[] hashEsperado;
        try
        {
            salt = Convert.FromBase64String(partes[1]);
            hashEsperado = Convert.FromBase64String(partes[2]);
        }
        catch (FormatException)
        {
            return false;
        }

        var hashCalculado = Rfc2898DeriveBytes.Pbkdf2(password, salt, iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);

        return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
    }
}