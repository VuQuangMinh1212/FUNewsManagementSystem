using FUNewsManagementSystem.BLL.Interfaces;
using FUNewsManagementSystem.DAL.Interfaces;
using FUNewsManagementSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FUNewsManagementSystem.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly INewsArticleRepository _newsArticleRepository;

        public CategoryService(ICategoryRepository categoryRepository, INewsArticleRepository newsArticleRepository)
        {
            _categoryRepository = categoryRepository;
            _newsArticleRepository = newsArticleRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllCategoriesAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(short id)
        {
            return await _categoryRepository.GetCategoryByIdAsync(id);
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await _categoryRepository.UpdateCategoryAsync(category);
        }

        public async Task DeleteCategoryAsync(short id)
        {
            await _categoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<bool> HasNewsInCategoryAsync(int categoryId)
        {
            return await _newsArticleRepository.HasNewsInCategoryAsync(categoryId);
        }

    }
}
