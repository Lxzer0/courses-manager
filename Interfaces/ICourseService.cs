using CoursesManager.Dtos;

namespace CoursesManager.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseReadDto>> GetAllAsync(Guid? currentUserId);
        Task<CourseReadDto?> GetByIdAsync(Guid id, Guid? currentUserId);
        Task<(byte[] Image, string Mime)?> GetImageAsync(Guid id);
        Task<CourseReadDto?> CreateAsync(CourseCreateDto createDto, Guid userId, string username);
        Task<bool> UpdateAsync(Guid id, CourseUpdateDto updateDto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId);
    }
}