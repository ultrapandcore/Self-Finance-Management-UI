using SelfFinanceManagerUI.Helpers.Attributes;

namespace SelfFinanceManagerUI.Data.Models
{
    public class DateRangeModel
    {
        [NoFutureDate]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [NoFutureDate]
        [NotLessThan(nameof(StartDate))]
        public DateTime? EndDate { get; set; }
    }
}
