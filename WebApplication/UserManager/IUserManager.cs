using MongoDB.Driver;

namespace WebApplication
{
    public interface IUserManager
    {
        public User CurrentUser { get; set; }
    }
}