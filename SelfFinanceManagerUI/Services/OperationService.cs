using Newtonsoft.Json;
using SelfFinanceManagerUI.Data.Models;
using SelfFinanceManagerUI.Helpers.Constants;
using SelfFinanceManagerUI.Services.Interfaces;

namespace SelfFinanceManagerUI.Services
{
    public class OperationService : IOperationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OperationService> _logger;

        public OperationService(IHttpClientFactory clientFactory, ILogger<OperationService> logger)
        {
            _httpClient = clientFactory.CreateClient(ApiConstants.ClientName);
            _logger = logger;
        }

        public async Task<IEnumerable<FinancialOperation>> GetFinancialOperationsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<FinancialOperation>>(ApiConstants.Operations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching financial operations");
                throw;
            }
        }

        public async Task<OperationsByDate> GetOperationsByDateAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                string apiUrl = $"{ApiConstants.OperationsByDateRange}/{startDate:yyyy.MM.dd}/{endDate:yyyy.MM.dd}";
                return await GetDateOperationsFromApi(apiUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching operations by date range ({startDate} - {endDate})");
                throw;
            }
        }

        public async Task<FinancialOperation> CreateOperationAsync(SaveOperationDto operationDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiConstants.Operations, operationDto);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<FinancialOperation>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating financial operation");
                throw;
            }
        }

        public async Task<FinancialOperation> UpdateOperationAsync(int id, SaveOperationDto operationDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{ApiConstants.Operations}/{id}", operationDto);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<FinancialOperation>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating financial operation with ID {id}");
                throw;
            }
        }

        public async Task<FinancialOperation> DeleteOperationAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiConstants.Operations}/{id}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<FinancialOperation>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting financial operation with ID {id}");
                throw;
            }
        }

        private async Task<OperationsByDate> GetDateOperationsFromApi(string apiUrl)
        {
            try
            {
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        return JsonConvert.DeserializeObject<OperationsByDate>(content);
                    }
                }

                // Return an empty object in case of an empty response
                return new OperationsByDate();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching date operations from API {apiUrl}");
                throw;
            }
        }
    }
}