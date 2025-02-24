﻿using FUNewsManagementSystem.DAL.Models;


namespace FUNewsManagementSystem.BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(short id);
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(short id);
        Task<bool> HasNewsInCategoryAsync(int categoryId);
    }
}
