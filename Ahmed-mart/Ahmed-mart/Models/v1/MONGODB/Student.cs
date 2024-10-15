using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Ahmed_mart.Models.v1.MONGODB
{
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;
        [BsonElement("gender")]
        public string Gender { get; set; } = string.Empty;
        [BsonElement("age")]
        public int Age { get; set; }
        [BsonElement("graduate")]
        public bool IsGraduate { get; set; }
        [BsonElement("courses")]
        public string[]? Courses { get; set; }
        public double? FileSize { get; set; }
        public string? ContentType { get; set; }
        public byte[]? Content { get; set; }
        public string? Base64File { get; set; }
    }
}
