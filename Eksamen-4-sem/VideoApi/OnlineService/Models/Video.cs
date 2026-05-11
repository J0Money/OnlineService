using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace OnlineService.Models;

public class Video
{
        
        [BsonId]
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; }
    
}