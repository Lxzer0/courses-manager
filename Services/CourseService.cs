using CoursesManager.Dtos;
using CoursesManager.Helpers;
using CoursesManager.Interfaces;
using CoursesManager.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace CoursesManager.Services
{
    public class CourseService : ICourseService
    {
        private readonly CoursesManagerContext _context;

        public CourseService(CoursesManagerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourseReadDto>> GetAllAsync(Guid? currentUserId)
        {
            var courses = await _context.Courses.ToListAsync();
            var result = new List<CourseReadDto>();

            foreach (var course in courses)
            {
                var user = await _context.Users.FindAsync(course.UserId);
                if (user == null) throw new InvalidOperationException("Owner user not found");

                bool isOwner = currentUserId.HasValue && course.UserId == currentUserId.Value;
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

            return result;
        }

        public async Task<CourseReadDto?> GetByIdAsync(Guid id, Guid? currentUserId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return null;

            var user = await _context.Users.FindAsync(course.UserId);
            if (user == null) return null;

            bool isOwner = currentUserId.HasValue && course.UserId == currentUserId.Value;

            return new CourseReadDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                CategoryId = course.CategoryId,
                Username = user.Username,
                Owner = isOwner
            };
        }

        public async Task<(byte[] Image, string Mime)?> GetImageAsync(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null || course.Image == null || string.IsNullOrEmpty(course.Mime)) return null;

            return (course.Image, course.Mime);
        }

        public async Task<CourseReadDto?> CreateAsync(CourseCreateDto createDto, Guid userId, string username)
        {
            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await createDto.Image.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            var mimeType = MimeHelper.GetMimeType(imageBytes);
            if (mimeType == "application/octet-stream") return null;

            var newCourse = new Course
            {
                Id = Guid.NewGuid(),
                Title = createDto.Title,
                Description = createDto.Description,
                Image = imageBytes,
                Mime = mimeType,
                CategoryId = createDto.CategoryId,
                UserId = userId
            };

            _context.Courses.Add(newCourse);
            await _context.SaveChangesAsync();

            return new CourseReadDto
            {
                Id = newCourse.Id,
                Title = newCourse.Title,
                Description = newCourse.Description,
                CategoryId = newCourse.CategoryId,
                Username = username,
                Owner = true
            };
        }

        public async Task<bool> UpdateAsync(Guid id, CourseUpdateDto updateDto, Guid userId)
        {
            var existingCourse = await _context.Courses.FindAsync(id);
            if (existingCourse == null) return false;
            if (existingCourse.UserId != userId) return false;

            if (updateDto.Title != null) existingCourse.Title = updateDto.Title;
            if (updateDto.Description != null) existingCourse.Description = updateDto.Description;
            if (updateDto.CategoryId.HasValue) existingCourse.CategoryId = updateDto.CategoryId.Value;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return false;
            if (course.UserId != userId) return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}