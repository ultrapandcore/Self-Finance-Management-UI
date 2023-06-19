using SelfFinanceManagerUI.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SelfFinanceManagerUI.Helpers.Attributes
{
    public class UniqueCategoryNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var categoryName = value as string;
            var categoryService = validationContext.GetService<ICategoryService>();

            var isUnique = Task.Run(() => categoryService.IsCategoryNameUniqueAsync(categoryName)).Result;

            return isUnique ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }
    }
}
