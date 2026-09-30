using BCrypt.Net;

namespace Market.Extensions;

public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    public static bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(providedPassword) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}