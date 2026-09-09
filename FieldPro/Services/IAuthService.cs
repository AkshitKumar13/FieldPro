using TaskForge.Models;

namespace TaskForge.Services;

public interface IAuthService
{
    Task<User?> LoginAsync(string email);

    Task LogoutAsync();

    Task<bool> IsAuthenticatedAsync();
}