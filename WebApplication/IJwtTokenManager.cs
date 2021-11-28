namespace WebApplication
{
    public interface IJwtTokenManager
    {
        string Authenticate(string userName, string password);
    }
}