using Bunit;
using SelfFinanceManagerUI.Data.Models;
using SelfFinanceManagerUI.Pages;
using SelfFinanceManagerUI.Shared.Components;
using Xunit;

namespace SelfFinanceManager.UnitTests
{
    public class OperationsPageTests : TestsBase
    {
        private IRenderedComponent<Operations> _cut;

        public OperationsPageTests()
        {
            _cut = _testContext.RenderComponent<Operations>();
        }

        [Fact]
        public void CreateOperation_ValidData_CreatesNewOperation()
        {
            // Arrange
            var operationName = "New Operation";
            var categoryId = 1;
            var amount = "100";
            var date = DateTime.Now.ToString("yyyy-MM-dd");

            // Act
            var operationForm = _cut.FindComponent<OperationForm<SaveOperationDto>>();
            operationForm.Find("input[placeholder='Enter name']").Change(operationName);
            operationForm.Find("select").Change(categoryId);
            operationForm.Find("input[placeholder='Enter amount']").Change(amount);
            operationForm.Find("input[type='date']").Change(date);
            operationForm.Find("button[type='submit']").Click();

            // Assert
            var createdOperation = _operations.Find(o => o.Name == operationName);
            Assert.NotNull(createdOperation);
        }

        [Fact]
        public void UpdateOperation_ValidData_UpdatesOperation()
        {
            // Arrange
            var updateButtons = _cut.FindAll(".btn.btn-warning.me-2.btn-custom-width");
            Assert.NotEmpty(updateButtons); // Make sure we have at least one button

            var updateForm = _cut.Find("div.card-body");
            Assert.NotNull(updateForm);

            // Act
            var operationName = "Updated Operation";
            var categoryId = 2;
            var amount = "200";
            var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            updateButtons[0].Click();
            _cut.WaitForState(() => _cut.Instance.IsEditing);

            updateForm.QuerySelector("input[placeholder='Enter name']").Change(operationName);
            updateForm.QuerySelector("select").Change(categoryId);
            updateForm.QuerySelector("input[placeholder='Enter amount']").Change(amount);
            updateForm.QuerySelector("input[type='date']").Change(date);
            updateForm.QuerySelector("button[type='submit']").Click();

            // Assert
            _cut.WaitForAssertion(() =>
            {
                var updatedOperation = _operations.Find(o => o.Name == operationName);
                Assert.NotNull(updatedOperation);
            });
        }

        [Fact]
        public void OperationIsDeleted_WhenDeleteButtonClicked()
        {
            // Arrange
            var deleteButton = _cut.FindAll(".btn-danger.btn-custom-width")[0];
            var initialOperationCount = _cut.Instance.OperationsList.Count;

            // Act
            deleteButton.Click();

            // Assert
            _cut.WaitForAssertion(() =>
            {
                Assert.Equal(initialOperationCount - 1, _cut.Instance.OperationsList.Count); // One operation should be deleted
                Assert.DoesNotContain(_cut.Instance.OperationsList, o => o.Name == "Operation 1");
            });
        }
    }
}
