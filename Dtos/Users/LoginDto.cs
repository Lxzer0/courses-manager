using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CoursesManager.Dtos
{
    public class LoginDto
    {
        [JsonPropertyName("login")]
        public required string Email { get; set; }
        [JsonPropertyName("pass")]
        public required string Password { get; set; }
    }
}