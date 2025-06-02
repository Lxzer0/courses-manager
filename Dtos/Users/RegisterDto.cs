using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoursesManager.Dtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("login")]
        public required string Email { get; set; }
        [Required]
        [JsonPropertyName("pass")]
        public required string Password { get; set; }
        [Required]
        [JsonPropertyName("name")]
        public required string Username { get; set; }
    }
}