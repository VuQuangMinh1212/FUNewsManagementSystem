using FUNewsManagementSystem.DAL.Models;
using FUNewsManagementSystem.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DAL.Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        private readonly FunewsManagementContext _context;

        public NewsArticleRepository(FunewsManagementContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<NewsArticle>> GetAllAsync()
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .ToListAsync();
        }


        public async Task<IEnumerable<NewsArticle>> GetAllActiveAsync()
        {
            return await _context.NewsArticles
                .Where(n => n.NewsStatus == true)
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .ToListAsync();
        }




        public async Task<NewsArticle> GetByIdAsync(string id)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);
        }


        public async Task CreateAsync(NewsArticle newsArticle)
        {
            _context.NewsArticles.Add(newsArticle);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(NewsArticle newsArticle)
        {
            _context.NewsArticles.Update(newsArticle);
            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync(string id)
        {
            var newsArticle = await _context.NewsArticles.FindAsync(id);
            if (newsArticle != null)
            {
                _context.NewsArticles.Remove(newsArticle);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<NewsArticle>> GetFilteredNewsArticlesAsync(string searchTitle, int? categoryFilter)
        {
            var query = _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTitle))
            {
                query = query.Where(n => n.NewsTitle.Contains(searchTitle));
            }

            if (categoryFilter.HasValue)
            {
                query = query.Where(n => n.CategoryId == categoryFilter);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> HasNewsInCategoryAsync(int categoryId)
        {
            return await _context.NewsArticles.AnyAsync(article => article.CategoryId == categoryId);
        }

        public async Task<List<NewsArticle>> GetNewsArticlesByStaffIdAsync(short staffId)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Where(n => n.CreatedById == staffId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }
    }
}
