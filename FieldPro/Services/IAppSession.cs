namespace FieldPro.Services;

public interface IAppSession
{
    bool IsAuthenticated { get; }

    void Login();

    void Logout();
}
