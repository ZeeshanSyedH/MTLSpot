using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MTLSpotScraper
{
    public interface IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string Id { get; set; }
    }
}