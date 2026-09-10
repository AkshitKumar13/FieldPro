namespace FieldPro.Services;

public interface IAuthService
{
    Task InitializeAsync();

    Task<bool> LoginAsync(string email, string password);

    Task LogoutAsync();

    Task<bool> IsAuthenticatedAsync();
}