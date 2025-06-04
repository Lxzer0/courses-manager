using CoursesManager.Dtos;
using CoursesManager.Models;

namespace CoursesManager.Interfaces
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<bool> EmailExistsAsync(string email);
        Task<User> RegisterAsync(RegisterDto registerDto);
    }
}