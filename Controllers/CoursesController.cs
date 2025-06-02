using CoursesManager.Models;
using CoursesManager.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CoursesManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly CoursesManagerContext _context;

        public CoursesController(CoursesManagerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseReadDto>>> GetCourses()
        {
            var courses = await _context.Courses.ToListAsync();

            var result = new List<CourseReadDto>();

            foreach (var course in courses)
            {
                var category = await _context.Categories.FindAsync(course.CategoryId);
                var user = await _context.Users.FindAsync(course.UserId);

                if (category == null || user == null) continue;

                result.Add(new CourseReadDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    Category = category.Name,
                    Username = user.Username
                });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseReadDto>> GetCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);

            if (course == null) return NotFound(new { error = "Course not found" });

            var category = await _context.Categories.FindAsync(course.CategoryId);
            var user = await _context.Users.FindAsync(course.UserId);

            if (category == null || user == null) return NotFound(new { error = "Related data not found" });

            var result = new CourseReadDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Category = category.Name,
                Username = user.Username
            };

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> PostCourse([FromBody] CourseCreateDto course)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("uuid");
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            var newCourse = new Course
            {
                Id = Guid.NewGuid(),
                Title = course.Title,
                Description = course.Description,
                CategoryId = course.CategoryId,
                UserId = userId,
            };
            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCourse), new { id = newCourse.Id }, newCourse);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(Guid id, [FromBody] CourseUpdateDto course)
        {
            var userIdClaim = User.FindFirst("uuid");
            if (userIdClaim == null) return Unauthorized();

            var existingCourse = await _context.Courses.FindAsync(id);
            if (existingCourse == null) return NotFound(new { error = "Course not found" });

            var userId = Guid.Parse(userIdClaim.Value);
            if (existingCourse.UserId != userId) return Forbid();

            if (course.Title != null) existingCourse.Title = course.Title;
            if (course.Description != null) existingCourse.Description = course.Description;
            if (course.CategoryId.HasValue) existingCourse.CategoryId = course.CategoryId.Value;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var userIdClaim = User.FindFirst("uuid");
            if (userIdClaim == null) return Unauthorized();

            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(new { error = "Course not found" });

            var userId = Guid.Parse(userIdClaim.Value);
            if (course.UserId != userId) return Forbid();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
