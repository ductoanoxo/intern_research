using System;
using System.Security.Cryptography;
using DddCqrsJwtDemo.Application.Abstractions;

namespace DddCqrsJwtDemo.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128-bit
    private const int KeySize = 32;  // 256-bit
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(password));
        }

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, KeySize);

        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        var parts = passwordHash.Split('.', 3);
        if (parts.Length != 3)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out var iterations))
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[1]);
        var hash = Convert.FromBase64String(parts[2]);

        var testHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithm, hash.Length);

        return CryptographicOperations.FixedTimeEquals(hash, testHash);
    }
}
