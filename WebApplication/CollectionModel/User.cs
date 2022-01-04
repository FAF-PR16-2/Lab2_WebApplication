using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication
{
    public class User
    {
        [BsonId] public ObjectId Id { get; set; }
        
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}