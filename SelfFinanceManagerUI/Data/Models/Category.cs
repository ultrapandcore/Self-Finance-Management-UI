namespace SelfFinanceManagerUI.Data.Models
{
    public class Category
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public bool IsExpanded { get; set; } = false;
        public List<FinancialOperation> Operations { get; set; } = new();
    }
}
