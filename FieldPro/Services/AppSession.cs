using TaskForge.Models;

namespace TaskForge.Services;

public class AppSession : IAppSession
{
    public bool IsAuthenticated => CurrentUser is not null;

    public User? CurrentUser { get; private set; }

    public void Login(User user)
    {
        CurrentUser = user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }
}
