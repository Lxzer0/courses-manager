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
        [JsonPropertyName("category_id")]
        public Guid? CategoryId { get; set; }
    }
}