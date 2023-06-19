using SelfFinanceManagerUI.Data.Models;

namespace SelfFinanceManagerUI.Services.Interfaces
{
    public interface IOperationService
    {
        Task<IEnumerable<FinancialOperation>> GetFinancialOperationsAsync();
        Task<OperationsByDate> GetOperationsByDateAsync(DateTime startDate, DateTime endDate);
        Task<FinancialOperation> CreateOperationAsync(SaveOperationDto operationDto);
        Task<FinancialOperation> UpdateOperationAsync(int id, SaveOperationDto operationDto);
        Task<FinancialOperation> DeleteOperationAsync(int id);
    }
}
