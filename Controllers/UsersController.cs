using CoursesManager.Models;
using CoursesManager.Interfaces;
using CoursesManager.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CoursesManager.Controllers
{
    [ApiController]
    [Route("api")]
    public class UsersController : ControllerBase
    {
        private readonly CoursesManagerContext _context;
        private readonly IJwtService _jwtService;

        public UsersController(CoursesManagerContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == login.Email);

            if (user != null && user.Password == login.Password)
            {
                var jwt = _jwtService.GenerateJwtToken(user.Id);

                Response.Cookies.Append("jwt", jwt, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    Expires = DateTime.UtcNow.AddHours(1)
                });

                return Ok(new { token = jwt });
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

            if (_context.Users.Any(u => u.Email == register.Email)) return BadRequest(new { error = "Email already exists" });

            if (HttpContext.Request.Cookies.ContainsKey("jwt")) return StatusCode(403, new { error = "Already logged in" });

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = register.Email,
                Password = register.Password,
                Username = register.Username
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var jwt = _jwtService.GenerateJwtToken(newUser.Id);

            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return Ok(new { token = jwt });
        }
    }
}
