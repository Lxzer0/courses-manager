using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoursesManager.Dtos
{
    public class CourseReadDto
    {
        public required Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required string Username { get; set; }
    }
}