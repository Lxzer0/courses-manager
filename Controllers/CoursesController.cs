using CoursesManager.Dtos;
using CoursesManager.Helpers;
using CoursesManager.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoursesManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseReadDto>>> GetCourses()
        {
            var userIdClaim = User.FindFirst("uuid");
            Guid? currentUserId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
            var result = await _courseService.GetAllAsync(currentUserId);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseReadDto>> GetCourse(Guid id)
        {
            var userIdClaim = User.FindFirst("uuid");
            Guid? currentUserId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
            var result = await _courseService.GetByIdAsync(id, currentUserId);
            if (result == null) return NotFound(new { error = "Course not found" });

            return Ok(result);
        }

        [HttpGet("image/{id}")]
        public async Task<ActionResult> GetCourseImage(Guid id)
        {
            var imageData = await _courseService.GetImageAsync(id);
            if (imageData == null) return NotFound(new { error = "Course not found" });

            var (image, mimeType) = imageData.Value;
            if (image == null || image.Length == 0) return NotFound(new { error = "Image not found" });

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
            var username = User.FindFirst("username")?.Value;
            if (username == null) return StatusCode(StatusCodes.Status500InternalServerError);

            CourseReadDto? courseDto = await _courseService.CreateAsync(course, userId, username);
            if (courseDto == null) return BadRequest();

            return CreatedAtAction(nameof(GetCourse), new { id = courseDto.Id }, courseDto);
        }

        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCourse(Guid id, [FromBody] CourseUpdateDto course)
        {
            var userIdClaim = User.FindFirst("uuid");
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);
            var updated = await _courseService.UpdateAsync(id, course, userId);
            if (!updated) return NotFound("Course not found or not owner");

            return Ok();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            var userIdClaim = User.FindFirst("uuid");
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim.Value);
            var deleted = await _courseService.DeleteAsync(id, userId);
            if (!deleted) return NotFound("Course not found or not owner");

            return Ok();
        }
    }
}
