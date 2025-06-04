using CoursesManager.Interfaces;
using CoursesManager.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CoursesManager.Controllers
{
    [ApiController]
    [Route("api")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;

        public UsersController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var user = await _userService.AuthenticateAsync(login.Email, login.Password);

            if (user != null)
            {
                var jwt = _jwtService.GenerateJwtToken(user.Id, user.Username);

                Response.Cookies.Append("jwt", jwt, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                return Ok();
            }

            return Unauthorized(new { error = "Invalid email or password" });
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt", new CookieOptions
            {
                HttpOnly = true,
                Secure = false
            });

            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (await _userService.EmailExistsAsync(register.Email)) return BadRequest(new { error = "Email already exists" });

            if (HttpContext.Request.Cookies.ContainsKey("jwt")) return StatusCode(StatusCodes.Status403Forbidden, new { error = "Already logged in" });

            var newUser = await _userService.RegisterAsync(register);

            var jwt = _jwtService.GenerateJwtToken(newUser.Id, newUser.Username);

            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return Ok();
        }
    }
}
