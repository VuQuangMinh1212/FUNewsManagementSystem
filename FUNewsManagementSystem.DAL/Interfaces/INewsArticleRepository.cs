using FUNewsManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.DAL.Interfaces
{
    public interface INewsArticleRepository
    {
        Task<IEnumerable<NewsArticle>> GetAllAsync();
        Task<IEnumerable<NewsArticle>> GetAllActiveAsync();
        Task<NewsArticle> GetByIdAsync(string id);
        Task CreateAsync(NewsArticle newsArticle);
        Task UpdateAsync(NewsArticle newsArticle);
        Task DeleteAsync(string id);
        Task<IEnumerable<NewsArticle>> GetFilteredNewsArticlesAsync(string searchTitle, int? categoryFilter);
        Task<bool> HasNewsInCategoryAsync(int categoryId);
        Task<List<NewsArticle>> GetNewsArticlesByStaffIdAsync(short staffId);
    }
}

