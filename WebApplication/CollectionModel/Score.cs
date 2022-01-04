using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication
{
    public class Score
    {
        [BsonId] public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        
        public string nickName { get; set; }
        public string score { get; set; }
    }
}