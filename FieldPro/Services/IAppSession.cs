using TaskForge.Models;

namespace TaskForge.Services;

public interface IAppSession
{
    bool IsAuthenticated { get; }

    User? CurrentUser { get; }

    void Login(User user);

    void Logout();
}