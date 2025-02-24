using FUNewsManagementSystem.DAL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;

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
                .Include(n => n.NewsTags)
                .ThenInclude(n => n.Tag)
                .ToListAsync();
        }


        public async Task<IEnumerable<NewsArticle>> GetAllActiveAsync()
        {
            return await _context.NewsArticles
                .Where(n => n.NewsStatus == true)
                .Include(n => n.Category)
                .Include(n => n.NewsTags)
                .ThenInclude(n => n.Tag)
                .ToListAsync();
        }




        public async Task<NewsArticle> GetByIdAsync(string id)
        {
            return await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.NewsTags)
                .ThenInclude(n => n.Tag)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);
        }


        public async Task CreateAsync(NewsArticle newsArticle, List<int> tags)
        {
            _context.NewsArticles.Add(newsArticle);
            if (tags != null && tags.Any())
            {
                var newsTags = tags.Select(tagId => new NewsTag
                {
                    NewsArticleId = newsArticle.NewsArticleId,
                    TagId = tagId
                }).ToList();
                _context.NewsTag.AddRange(newsTags);
            }
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(NewsArticle updatedArticle, List<int> tags)
        {
            var existingArticle = await _context.NewsArticles
        .Include(n => n.NewsTags)
        .FirstOrDefaultAsync(n => n.NewsArticleId == updatedArticle.NewsArticleId);

            if (existingArticle == null)
                throw new Exception("NewsArticle not found.");

            _context.Entry(existingArticle).CurrentValues.SetValues(updatedArticle);

            if (tags != null)
            {
                var tagsToRemove = existingArticle.NewsTags.Where(nt => !tags.Contains(nt.TagId)).ToList();
                _context.NewsTag.RemoveRange(tagsToRemove);

                foreach (var tagId in tags)
                {
                    if (!existingArticle.NewsTags.Any(nt => nt.TagId == tagId))
                    {
                        existingArticle.NewsTags.Add(new NewsTag
                        {
                            NewsArticleId = existingArticle.NewsArticleId,
                            TagId = tagId
                        });
                    }
                }
            }
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

        public async Task<IEnumerable<NewsArticle>> GetLatestNews(int count)
        {
            return await _context.NewsArticles
                .Where(n => n.NewsStatus == true)
                .OrderByDescending(n => n.CreatedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsReportAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.NewsArticles.AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate <= endDate.Value);
            }

            return await query
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

    }
}
