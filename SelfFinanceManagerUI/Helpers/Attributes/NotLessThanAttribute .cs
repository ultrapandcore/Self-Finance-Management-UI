using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace SelfFinanceManagerUI.Helpers.Attributes
{
    public class NotLessThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public NotLessThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var currentValue = (DateTime)value;
            var comparisonValue = GetComparisonValue(validationContext);

            if (comparisonValue.HasValue && currentValue < comparisonValue.Value)
            {
                var localizer = validationContext.GetService(typeof(IStringLocalizer<App>)) as IStringLocalizer;
                var errorMessage = localizer["{0} cannot be less than {1}"].Value;
                return new ValidationResult(FormatErrorMessage(errorMessage, validationContext.DisplayName, _comparisonProperty));
            }

            return ValidationResult.Success;
        }

        private DateTime? GetComparisonValue(ValidationContext validationContext)
        {
            var propertyInfo = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property with name {_comparisonProperty} not found on {validationContext.ObjectType.Name}");
            }

            var comparisonValue = propertyInfo.GetValue(validationContext.ObjectInstance);
            if (comparisonValue == null)
            {
                return null;
            }

            return (DateTime)comparisonValue;
        }

        public string FormatErrorMessage(string errorMessage, string displayName, string comparisonProperty)
        {
            return string.Format(errorMessage, displayName, comparisonProperty);
        }
    }
}
