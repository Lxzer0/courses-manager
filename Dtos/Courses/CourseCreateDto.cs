using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoursesManager.Dtos
{
    public class CourseCreateDto
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Description { get; set; }
        [Required]
        public required IFormFile Image { get; set; }
        [Required]
        public required Guid CategoryId { get; set; }
    }
}