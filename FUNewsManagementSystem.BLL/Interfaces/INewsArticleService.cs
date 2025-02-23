﻿using FUNewsManagementSystem.DAL.Models;

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
        Task<IEnumerable<NewsArticle>> GetLatestNews(int count);
    }
}
