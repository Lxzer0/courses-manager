using CoursesManager.Models;
using CoursesManager.Dtos;
using CoursesManager.Helpers;
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
                var user = await _context.Users.FindAsync(course.UserId);
                if (user == null) return StatusCode(StatusCodes.Status500InternalServerError);

                var userIdClaim = User.FindFirst("uuid");
                bool isOwner = false;
                if (userIdClaim != null) isOwner = course.UserId == Guid.Parse(userIdClaim.Value);

                result.Add(new CourseReadDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Description = course.Description,
                    CategoryId = course.CategoryId,
                    Username = user.Username,
                    Owner = isOwner
                });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseReadDto>> GetCourse(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(new { error = "Course not found" });

            var user = await _context.Users.FindAsync(course.UserId);
            if (user == null) return StatusCode(StatusCodes.Status500InternalServerError);

            var userIdClaim = User.FindFirst("uuid");
            bool isOwner = false;
            if (userIdClaim != null) isOwner = course.UserId == Guid.Parse(userIdClaim.Value);

            var result = new CourseReadDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                CategoryId = course.CategoryId,
                Username = user.Username,
                Owner = isOwner
            };

            return Ok(result);
        }

        [HttpGet("image/{id}")]
        public async Task<ActionResult> GetCourseImage(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound(new { error = "Course not found" });

            var image = course.Image;
            if (image == null || image.Length == 0) return NotFound(new { error = "Image not found" });

            var mimeType = course.Mime;

            Response.Headers["Cache-Control"] = "public, max-age=604800, immutable";

            return File(image, mimeType);
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> PutCourse([FromForm] CourseCreateDto course)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userIdClaim = User.FindFirst("uuid");
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);

            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await course.Image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            var mimeType = MimeHelper.GetMimeType(imageBytes);
            if (mimeType == "application/octet-stream") BadRequest(new { error = "Unrecognized image format" });

            var newCourse = new Course
            {
                Id = Guid.NewGuid(),
                Title = course.Title,
                Description = course.Description,
                Image = imageBytes,
                Mime = mimeType,
                CategoryId = course.CategoryId,
                UserId = userId,
            };

            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            var username = User.FindFirst("username")?.Value;
            if (username == null) return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Username not found in claims" });

            var courseDto = new CourseReadDto
            {
                Id = newCourse.Id,
                Title = newCourse.Title,
                Description = newCourse.Description,
                CategoryId = newCourse.CategoryId,
                Username = username,
                Owner = true
            };

            return CreatedAtAction(nameof(GetCourse), new { id = courseDto.Id }, courseDto);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCourse(Guid id, [FromBody] CourseUpdateDto course)
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
