using TaskForge.Models;
using TaskForge.Data;

namespace TaskForge.Services;

public class AuthService : IAuthService
{
    private readonly TaskForgeDatabase _database;

    public AuthService(TaskForgeDatabase database)
    {
        _database = database;
    }

    public async Task<User?> LoginAsync(string email)
    {
        return (await _database.GetUsersAsync()).FirstOrDefault(user =>
            user.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public Task LogoutAsync() => Task.CompletedTask;

    public Task<bool> IsAuthenticatedAsync() => Task.FromResult(false);
}