using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public interface IAuthService
{
    Task<User?> LoginAsync(string email);

    Task LogoutAsync();

    Task<bool> IsAuthenticatedAsync();
}