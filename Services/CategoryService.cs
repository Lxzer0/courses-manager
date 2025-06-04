using CoursesManager.Interfaces;
using CoursesManager.Models;
using Microsoft.EntityFrameworkCore;

namespace CoursesManager.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly CoursesManagerContext _context;

        public CategoryService(CoursesManagerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}