using System.Security.Cryptography;
using System.Text;

namespace Soltec.Security;

public static class SecurityService
{
    /// <summary>
    /// Crea un hash SHA256. 
    /// Nota: Para producción, considera Argon2 o BCrypt, 
    /// pero esta es la evolución directa y segura de tu método actual.
    /// </summary>
    public static string CreatePasswordHash(string pwd, string salt)
    {
        // Usamos la sintaxis de 'using' simplificada de C# 8+ 
        // y SHA256 en lugar de SHA1 (obsoleto).
        byte[] messageBytes = Encoding.UTF8.GetBytes(string.Concat(pwd, salt));
        
        // HashData es un método estático más eficiente introducido en versiones recientes
        byte[] hashedBytes = SHA256.HashData(messageBytes);
        
        return Convert.ToBase64String(hashedBytes);
    }

    /// <summary>
    /// Genera un Salt seguro utilizando RandomNumberGenerator (reemplaza a RNGCryptoServiceProvider)
    /// </summary>
    public static string CreateSalt(int size)
    {
        // En .NET 8, RandomNumberGenerator.GetBytes es la forma preferida y más segura
        byte[] buff = RandomNumberGenerator.GetBytes(size);
        
        return Convert.ToBase64String(buff);
    }
}