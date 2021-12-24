namespace WebApplication
{
    public interface IPasswordHasher
    {
        public string HashString(string password);

        public bool VerifyPassword(string password, string storedHash);
    }
}