using SelfFinanceManagerUI.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SelfFinanceManagerUI.Data.Models
{
    public class SaveCategoryDto
    {
        [Required(ErrorMessageResourceType = typeof(Resources.App),
            ErrorMessageResourceName = "CategoryNameRequired")]
        [StringLength(50, MinimumLength = 2, ErrorMessageResourceType = typeof(Resources.App),
            ErrorMessageResourceName = "CategoryNameLength")]
        [UniqueCategoryName(ErrorMessageResourceType = typeof(Resources.App),
            ErrorMessageResourceName = "CategoryNameExists")]
        public string Name { get; set; }
    }
}
