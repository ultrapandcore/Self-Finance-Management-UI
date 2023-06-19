namespace SelfFinanceManagerUI.Data.Models
{
    public class OperationsByDate
    {
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }
        public List<FinancialOperation> Operations { get; set; } = new();
    }
}
