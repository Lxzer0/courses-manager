using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoursesManager.Dtos
{
    public class CourseCreateDto
    {
        [Required]
        [JsonPropertyName("title")]
        public required string Title { get; set; }
        [Required]
        [JsonPropertyName("description")]
        public required string Description { get; set; }
        [Required]
        [JsonPropertyName("categoryId")]
        public Guid CategoryId { get; set; }
    }
}