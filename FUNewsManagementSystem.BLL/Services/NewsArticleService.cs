using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.DAL.Interfaces;
using FUNewsManagementSystem.DAL.Models;

namespace FUNewsManagementSystem.BLL.Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _newsArticleRepository;

        public NewsArticleService(INewsArticleRepository newsArticleRepository)
        {
            _newsArticleRepository = newsArticleRepository;
        }

        public async Task<IEnumerable<NewsArticle>> GetAllNewsArticlesAsync()
        {
            return await _newsArticleRepository.GetAllAsync();
        }

        public async Task<NewsArticle?> GetNewsArticleByIdAsync(string id)
        {
            return await _newsArticleRepository.GetByIdAsync(id);
        }

        public async Task AddNewsArticleAsync(NewsArticle article)
        {
            await _newsArticleRepository.CreateAsync(article);
        }

        public async Task UpdateNewsArticleAsync(NewsArticle article)
        {
            await _newsArticleRepository.UpdateAsync(article);
        }

        public async Task DeleteNewsArticleAsync(string id)
        {
            await _newsArticleRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<NewsArticle>> GetFilteredNewsArticlesAsync(string searchTitle, int? categoryFilter)
        {
            return await _newsArticleRepository.GetFilteredNewsArticlesAsync(searchTitle, categoryFilter);
        }

        public async Task<bool> HasNewsInCategoryAsync(short categoryId)
        {
            return await _newsArticleRepository.HasNewsInCategoryAsync(categoryId);
        }

        public async Task<List<NewsArticle>> GetNewsArticlesByStaffIdAsync(short staffId)
        {
            return await _newsArticleRepository.GetNewsArticlesByStaffIdAsync(staffId);
        }

        public async Task<IEnumerable<NewsArticle>> GetLatestNews(int count)
        {
            return await _newsArticleRepository.GetLatestNews(count);
        }

        public async Task<IEnumerable<NewsArticle>> GetNewsReportAsync(DateTime? startDate, DateTime? endDate)
        {
            return await _newsArticleRepository.GetNewsReportAsync(startDate, endDate);
        }

    }
}
