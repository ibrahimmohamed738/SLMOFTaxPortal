using System.Security.Cryptography;

namespace SLMOFTaxPortal.Helpers;

public static class PasswordHelper
{
    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(32);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');

        if (parts.Length != 2)
            return false;

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] hash = Convert.FromBase64String(parts[1]);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256);

        byte[] computedHash = pbkdf2.GetBytes(32);

        return CryptographicOperations.FixedTimeEquals(hash, computedHash);
    }
}
