using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoursesManager.Dtos
{
    public class CourseReadDto
    {
        [JsonPropertyName("id")]
        public required Guid Id { get; set; }
        [JsonPropertyName("title")]
        public required string Title { get; set; }
        [JsonPropertyName("description")]
        public required string Description { get; set; }
        [JsonPropertyName("category_id")]
        public required Guid CategoryId { get; set; }
        [JsonPropertyName("username")]
        public required string Username { get; set; }
        [JsonPropertyName("owner")]
        public required bool Owner { get; set; }
    }
}