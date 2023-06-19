using SelfFinanceManagerUI.Data.Models;

namespace SelfFinanceManagerUI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int id);
        Task<IEnumerable<FinancialOperation>> GetOperationsByCategoryIdAsync(int id);
        Task<Category> CreateCategoryAsync(SaveCategoryDto categoryDto);
        Task<Category> UpdateCategoryAsync(int id, SaveCategoryDto categoryDto);
        Task<Category> DeleteCategoryAsync(int id);
        Task<bool> IsCategoryNameUniqueAsync(string categoryName);
    }
}
