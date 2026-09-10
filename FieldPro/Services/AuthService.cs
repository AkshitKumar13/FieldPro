using FieldPro.Data;
using FieldPro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Storage;
using System.Security.Cryptography;
using System.Text.Json;

namespace FieldPro.Services;

public class AuthService : IAuthService
{
    private const string UserIdKey = "fieldpro.auth.user-id";
    private const string AuthSeedFileName = "auth-seed.json";
    private readonly IDbContextFactory<AuthDbContext> _dbContextFactory;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);

    public AuthService(IDbContextFactory<AuthDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task InitializeAsync()
    {
        await _initializationLock.WaitAsync();
        try
        {
            await using var db = await _dbContextFactory.CreateDbContextAsync();
            await db.Database.EnsureCreatedAsync();

            if (!await db.Users.AnyAsync())
            {
                var seed = await ReadAuthSeedAsync();
                var (hash, salt) = HashPassword(seed.Password);
                db.Users.Add(new User
                {
                    Email = seed.Email.Trim(),
                    PasswordHash = hash,
                    PasswordSalt = salt
                });
                await db.SaveChangesAsync();
            }
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private static async Task<AuthSeed> ReadAuthSeedAsync()
    {
        await using var stream = await FileSystem.OpenAppPackageFileAsync(AuthSeedFileName);
        var seed = await JsonSerializer.DeserializeAsync<AuthSeed>(stream);

        if (seed is null || string.IsNullOrWhiteSpace(seed.Email) || string.IsNullOrWhiteSpace(seed.Password))
        {
            throw new InvalidOperationException($"The {AuthSeedFileName} file must contain an email and password.");
        }

        return seed;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        await InitializeAsync();
        var normalizedEmail = email.Trim().ToUpperInvariant();
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        var user = await db.Users.SingleOrDefaultAsync(candidate =>
            candidate.Email.ToUpper() == normalizedEmail && candidate.IsActive);

        if (user is null || !VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
        {
            return false;
        }

        await SecureStorage.Default.SetAsync(UserIdKey, user.Id.ToString());
        return true;
    }

    public Task LogoutAsync()
    {
        SecureStorage.Default.Remove(UserIdKey);
        return Task.CompletedTask;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var userId = await SecureStorage.Default.GetAsync(UserIdKey);
        if (!int.TryParse(userId, out var parsedUserId))
        {
            return false;
        }

        await using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.Users.AnyAsync(user => user.Id == parsedUserId && user.IsActive);
    }

    private static (string Hash, string Salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            100_000,
            HashAlgorithmName.SHA256,
            32);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    private static bool VerifyPassword(string password, string encodedHash, string encodedSalt)
    {
        try
        {
            var salt = Convert.FromBase64String(encodedSalt);
            var expectedHash = Convert.FromBase64String(encodedHash);
            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                100_000,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private sealed record AuthSeed(string Email, string Password);
}