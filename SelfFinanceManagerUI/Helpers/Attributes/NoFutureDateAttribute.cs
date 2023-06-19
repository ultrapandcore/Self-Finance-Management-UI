using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace SelfFinanceManagerUI.Helpers.Attributes
{
    public class NoFutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            DateTime dateValue = (DateTime)value;

            if (dateValue > DateTime.Now)
            {
                var localizer = validationContext.GetService(typeof(IStringLocalizer<App>)) as IStringLocalizer;

                return new ValidationResult(localizer["Date cannot be in the future."]);
            }

            return ValidationResult.Success;
        }
    }
}
