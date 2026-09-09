using ContosoDashboard.Data;
using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public class AuthService : IAuthService
{
    private readonly ContosoDatabase _database;

    public AuthService(ContosoDatabase database) => _database = database;

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