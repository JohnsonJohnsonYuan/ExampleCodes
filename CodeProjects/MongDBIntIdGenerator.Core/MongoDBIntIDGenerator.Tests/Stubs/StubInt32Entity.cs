using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBIntIdGenerator.Tests.Stubs
{
    public class StubInt32Entity
    {
        [BsonId(IdGenerator = typeof(Int32IdGenerator))]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}