namespace SelfFinanceManagerUI.Helpers
{
    public static class FormattingHelpers
    {
        public static string FormatAmount(decimal amount, bool isIncome)
        {
            string formattedAmount = amount % 1 == 0 ? $"{amount:N0}" : $"{amount:N2}";
            string sign = isIncome ? "+" : "-" ;

            return $"{sign} {formattedAmount}";
        }
    }
}
