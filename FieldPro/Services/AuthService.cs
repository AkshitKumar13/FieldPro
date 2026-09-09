using TaskForge.Data;
using TaskForge.Models;

namespace TaskForge.Services;

public class AuthService : IAuthService
{
    private readonly TaskForgeDatabase _database;

    public AuthService(TaskForgeDatabase database) => _database = database;

    public async Task<User?> LoginAsync(string email)
    {
        return (await _database.GetUsersAsync()).FirstOrDefault(x => x.Email == email);
    }

    public Task LogoutAsync()
    {
        return Task.CompletedTask;
    }

    public Task<bool> IsAuthenticatedAsync()
    {
        return Task.FromResult(false);
    }
}