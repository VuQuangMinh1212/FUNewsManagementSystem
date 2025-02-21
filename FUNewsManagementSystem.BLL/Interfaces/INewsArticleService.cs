using FUNewsManagementSystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.BLL.Interfaces
{
    public interface INewsArticleService
    {
        Task<IEnumerable<NewsArticle>> GetAllNewsArticlesAsync();
        Task<NewsArticle?> GetNewsArticleByIdAsync(string id);
        Task AddNewsArticleAsync(NewsArticle article);
        Task UpdateNewsArticleAsync(NewsArticle article);
        Task DeleteNewsArticleAsync(string id);
        Task<IEnumerable<NewsArticle>> GetFilteredNewsArticlesAsync(string searchTitle, int? categoryFilter);

        Task<bool> HasNewsInCategoryAsync(short categoryId);
        Task<List<NewsArticle>> GetNewsArticlesByStaffIdAsync(short staffId);
    }
}
