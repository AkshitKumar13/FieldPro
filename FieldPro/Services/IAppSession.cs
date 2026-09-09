using ContosoDashboard.Models;

namespace ContosoDashboard.Services;

public interface IAppSession
{
    bool IsAuthenticated { get; }

    User? CurrentUser { get; }

    void Login(User user);

    void Logout();
}