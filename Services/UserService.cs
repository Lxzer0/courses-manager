using CoursesManager.Dtos;
using CoursesManager.Interfaces;
using CoursesManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CoursesManager.Services
{
    public class UserService : IUserService
    {
        private readonly CoursesManagerContext _context;

        public UserService(CoursesManagerContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && user.Password == password) return user;

            return null;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User> RegisterAsync(RegisterDto registerDto)
        {
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = registerDto.Email,
                Password = registerDto.Password,
                Username = registerDto.Username
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }
    }
}