using SelfFinanceManagerUI.Data.Models;
using SelfFinanceManagerUI.Helpers.Constants;
using SelfFinanceManagerUI.Services.Interfaces;

namespace SelfFinanceManagerUI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IHttpClientFactory httpClientFactory, ILogger<CategoryService> logger)
        {
            _httpClient = httpClientFactory.CreateClient(ApiConstants.ClientName);
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<Category>>(ApiConstants.Categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories");
                throw;
            }
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Category>($"{ApiConstants.Categories}/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching category with ID {id}");
                throw;
            }
        }

        public async Task<IEnumerable<FinancialOperation>> GetOperationsByCategoryIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<FinancialOperation>>
                                        ($"{ApiConstants.OperationsByCategory}/{id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching operations from category with ID {id}");
                throw;
            }
        }

        public async Task<Category> CreateCategoryAsync(SaveCategoryDto categoryDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiConstants.Categories, categoryDto);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<Category>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category");
                throw;
            }
        }

        public async Task<Category> UpdateCategoryAsync(int id, SaveCategoryDto categoryDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{ApiConstants.Categories}/{id}", categoryDto);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<Category>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating category with ID {id}");
                throw;
            }
        }

        public async Task<Category> DeleteCategoryAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiConstants.Categories}/{id}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<Category>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting category with ID {id}");
                throw;
            }
        }

        public async Task<bool> IsCategoryNameUniqueAsync(string categoryName)
        {
            try
            {
                var categories = await GetCategoriesAsync();
                return !categories.Any(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while trying to check is category {categoryName} unique");
                throw;
            }
        }
    }
}
