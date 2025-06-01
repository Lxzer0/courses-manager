using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoursesManager.Dtos
{
    public class LoginDto
    {
        [Required]
        [JsonPropertyName("login")]
        public required string Email { get; set; }
        [Required]
        [JsonPropertyName("pass")]
        public required string Password { get; set; }
    }
}