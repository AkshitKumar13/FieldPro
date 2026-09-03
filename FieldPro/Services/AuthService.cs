namespace FieldPro.Services;

public class AuthService : IAuthService
{
    private bool _isAuthenticated;

    public Task<bool> LoginAsync(string email, string password)
    {
        // Temporary mock authentication.
        // We will replace this with API/JWT authentication later.

        if (email == "admin@test.com" &&
            password == "Password123")
        {
            _isAuthenticated = true;
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task LogoutAsync()
    {
        _isAuthenticated = false;

        return Task.CompletedTask;
    }

    public Task<bool> IsAuthenticatedAsync()
    {
        return Task.FromResult(_isAuthenticated);
    }
}