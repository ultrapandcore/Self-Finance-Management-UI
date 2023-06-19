using SelfFinanceManagerUI.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SelfFinanceManagerUI.Data.Models
{
    public class SaveOperationDto 
    {
        [Required(ErrorMessageResourceType = typeof(Resources.App),
            ErrorMessageResourceName = "OperationNameRequired")]
        [StringLength(50, MinimumLength = 3, ErrorMessageResourceType = typeof(Resources.App),
            ErrorMessageResourceName = "OperationNameLength")]
        public string Name { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessageResourceType = typeof(Resources.App),
            ErrorMessageResourceName = "OperationAmountOutOfRange")]
        public decimal Amount { get; set; }

        [Required]
        [NoFutureDate]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        public bool IsIncome { get; set; } = false;

        [Required]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Resources.App),
            ErrorMessageResourceName = "OperationCategoryRequired")]
        public int CategoryId { get; set; }
    }
}
