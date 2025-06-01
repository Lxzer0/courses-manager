using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoursesManager.Dtos
{
    public class CourseUpdateDto
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("categoryId")]
        public Guid? CategoryId { get; set; }
        [JsonPropertyName("userId")]
        public Guid? UserId { get; set; }
    }
}