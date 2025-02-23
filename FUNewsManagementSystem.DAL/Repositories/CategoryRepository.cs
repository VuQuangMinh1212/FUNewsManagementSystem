using FUNewsManagementSystem.DAL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.DAL.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FunewsManagementContext _context;

        public CategoryRepository(FunewsManagementContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }


        public async Task<Category> GetCategoryByIdAsync(short id)
        {
            return await _context.Categories.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }


        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteCategoryAsync(short id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return;

            bool isUsed = await _context.NewsArticles.AnyAsync(n => n.CategoryId == id);
            if (isUsed) return;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
