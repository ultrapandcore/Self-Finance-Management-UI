using Bunit;
using SelfFinanceManagerUI.Data.Models;
using SelfFinanceManagerUI.Pages;
using SelfFinanceManagerUI.Shared.Components;
using Xunit;

namespace SelfFinanceManager.UnitTests
{
    public class CategoriesPageTests : TestsBase
    {
        private IRenderedComponent<Categories> _cut;

        public CategoriesPageTests()
        {
            _cut = _testContext.RenderComponent<Categories>();
        }

        [Fact]
        public void CantCreateCategoryWithEmptyName_ValidationMessageAppears()
        {
            // Arrange
            var categoryForm = _cut.FindComponent<CategoryForm<SaveCategoryDto>>();
            var newCategoryInput = categoryForm.Find("input");
            var createButton = _cut.Find("button[type='submit']");
            newCategoryInput.Change(string.Empty);

            // Act
            createButton.Click();

            // Assert
            var validationMessage = categoryForm.Find("li.validation-message");
            Assert.Contains("Category name is required.", validationMessage.TextContent);
        }

        [Fact]
        public void CantCreateCategoryWithExistingName_ValidationMessageAppears()
        {
            // Arrange
            var existingCategoryName = "Category 1";
            var categoryForm = _cut.FindComponent<CategoryForm<SaveCategoryDto>>();
            var newCategoryInput = categoryForm.Find("input");
            var createButton = _cut.Find("button[type='submit']");
            newCategoryInput.Change(existingCategoryName);

            // Act
            createButton.Click();

            // Assert
            var validationMessage = categoryForm.Find("li.validation-message");
            Assert.Contains("Category name already exists.", validationMessage.TextContent);
        }

        [Fact]
        public void CanCreateCategoryWithAppropriateNameWhenClickButton()
        {
            // Arrange
            var newCategoryName = "New Category";
            var initialCategoryCount = _categories.Count;
            var categoryForm = _cut.FindComponent<CategoryForm<SaveCategoryDto>>();
            var newCategoryInput = categoryForm.Find("input");
            var createButton = _cut.Find("button[type='submit']");
            newCategoryInput.Change(newCategoryName);

            // Act
            createButton.Click();

            // Assert
            Assert.Equal(initialCategoryCount + 1, _categories.Count);
            Assert.Contains(_categories, c => c.Name == newCategoryName);
        }

        [Fact]
        public void CategoryExpandsToShowOperations_WhenClicked()
        {
            // Arrange
            var firstCategoryName = _cut.Find(".category-name");
            firstCategoryName.Click();

            // Act
            _cut.WaitForState(() => _cut.Instance.CategoriesList.FirstOrDefault()?.IsExpanded == true);

            // Assert
            var firstCategory = _categories.FirstOrDefault();
            if (firstCategory != null)
            {
                var firstCategoryOperations = firstCategory.Operations;
                var operationElements = _cut.FindAll(".operation-name").ToList();

                Assert.Equal(firstCategoryOperations.Count, operationElements.Count);

                foreach (var operation in firstCategoryOperations)
                {
                    Assert.Contains(operationElements, oe => oe.TextContent == operation.Name);
                }
            }
        }

        [Fact]
        public void CategoryExpandsToShowNoOperationsMessage_WhenClicked()
        {
            // Arrange
            var thirdCategoryName = _cut.FindAll(".category-name")[2];
            thirdCategoryName.Click();

            // Act
            _cut.WaitForState(() => _cut.Instance.CategoriesList[2].IsExpanded == true);

            // Assert
            var noOperationsMessageElement = _cut.Find(".text-muted");
            Assert.Equal("No operations found for this category.", noOperationsMessageElement.TextContent);
        }

        [Fact]
        public void CategoryUpdateFormAppearsAndUpdateSucceeds_WhenUpdateButtonClicked()
        {
            // Arrange
            var firstUpdateButton = _cut.FindAll(".btn-warning.btn-custom-width")[0];
            firstUpdateButton.Click();

            // Act
            _cut.WaitForState(() => _cut.Instance.EditingCategoryId != 0);

            // Assert
            var updateForm = _cut.Find("div.card-body");
            Assert.NotNull(updateForm);

            var updatedCategoryName = "Updated Category 1";
            var categoryNameInput = updateForm.QuerySelector("input[type='text']");
            var saveButton = updateForm.QuerySelectorAll("button[type='submit']").FirstOrDefault(button => button.TextContent == "Save");
            categoryNameInput.Change(updatedCategoryName);

            // Act
            saveButton.Click();

            // Assert
            _cut.WaitForAssertion(() =>
            {
                var firstCategoryName = _cut.FindAll(".category-name")[0];
                Assert.Contains(updatedCategoryName, firstCategoryName.TextContent);
            });
        }

        [Fact]
        public void CategoryIsDeleted_WhenDeleteButtonClicked()
        {
            // Arrange
            var firstDeleteButton = _cut.FindAll(".btn-danger.btn-custom-width")[0];
            var initialCategoryCount = _cut.Instance.CategoriesList.Count;

            // Act
            firstDeleteButton.Click();

            // Assert
            _cut.WaitForAssertion(() =>
            {
                Assert.Equal(initialCategoryCount - 1, _cut.Instance.CategoriesList.Count); // One category should be deleted
                Assert.DoesNotContain(_cut.Instance.CategoriesList, c => c.Name == "Category 1");
            });
        }
    }
}
