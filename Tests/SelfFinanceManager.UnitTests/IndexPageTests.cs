using Bunit;
using Xunit;
using Index = SelfFinanceManagerUI.Pages.Index;

namespace SelfFinanceManager.UnitTests
{
    public class IndexPageTests : TestsBase
    {
        private IRenderedComponent<Index> _cut;

        public IndexPageTests()
        {
            _cut = _testContext.RenderComponent<Index>();
        }

        [Fact]
        public void RendersWelcomeMessageWithUsername()
        {
            // Arrange & Act
            var welcomeMessage = _cut.Find("h1");

            // Assert
            welcomeMessage.MarkupMatches($"<h1>Welcome, {TestUser}!</h1>");
        }

        [Fact]
        public async Task RendersOperationsWithCorrectIncomeAndExpenses()
        {
            // Arrange
            await _cut.InvokeAsync(() => _cut.Instance.LoadOperationsAsync());
            var operationItems = _cut.FindAll("li.operation-list").ToList();
            var incomeBadge = _cut.Find("div.operation-amount.badge.bg-success");
            var expensesBadge = _cut.Find("div.operation-amount.badge.bg-danger");

            // Act & Assert
            Assert.Equal(2, operationItems.Count);
            Assert.Contains("Income: + 100", incomeBadge.TextContent);
            Assert.Contains("Expenses: - 50", expensesBadge.TextContent);
        }

        [Fact]
        public async Task RendersNoOperationsMessageWhenNoOperationsFound()
        {
            // Arrange
            await _cut.InvokeAsync(() => _cut.Instance.LoadOperationsAsync());
            _cut.Instance.OperationsByDate = new();
            _cut.Render();

            // Act
            var noOperationsMessage = _cut.Find("div.text-muted");

            // Assert
            Assert.Equal("No operations found for this date.", noOperationsMessage.TextContent);
        }

        [Fact]
        public async Task RendersCorrectCategoryNamesForOperations()
        {
            // Arrange
            await _cut.InvokeAsync(() => _cut.Instance.LoadOperationsAsync());
            var operationItems = _cut.FindAll("li.operation-list").ToList();

            // Act & Assert
            Assert.Contains("Category 1", operationItems[0].TextContent);
            Assert.Contains("Category 2", operationItems[1].TextContent);
        }

        [Fact]
        public async Task HandlesInvalidDateRangesCorrectly()
        {
            // Arrange
            await _cut.InvokeAsync(() => _cut.Instance.LoadOperationsAsync());
            var startDateInput = _cut.Find("input#startDate");
            var endDateInput = _cut.Find("input#endDate");
            var submitButton = _cut.Find("button[type='submit']");

            // Act
            startDateInput.Change("2023-06-01");
            endDateInput.Change("2023-05-01");
            submitButton.Click();

            // Assert
            var validationMessage = _cut.Find("li.validation-message");
            Assert.Contains("Date cannot be in the future.", validationMessage.TextContent);
        }
    }  
}
