using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Moq;
using SelfFinanceManagerUI.Data.Models;
using SelfFinanceManagerUI.Services.Interfaces;
using System.Security.Claims;

namespace SelfFinanceManager.UnitTests
{
    public class TestsBase
    {
        public const string TestUser = "TEST USER";
        public TestContext _testContext;

        public List<FinancialOperation> _operations = new()
        {
            new FinancialOperation { Id = 1, Name = "Operation 1", Amount = 100, Date = DateTime.Today, IsIncome = true, CategoryId = 1 },
            new FinancialOperation { Id = 2, Name = "Operation 2", Amount = 50, Date = DateTime.Today, IsIncome = false, CategoryId = 2 }
        };

        public List<Category> _categories = new()
        {
            new Category { Id = 1, Name = "Category 1" },
            new Category { Id = 2, Name = "Category 2" },
            new Category { Id = 3, Name = "Category 3" }
        };

        public TestsBase()
        {
            _testContext = new TestContext();
            var authContext = _testContext.AddTestAuthorization().SetAuthorized(TestUser);
            authContext.SetClaims(new Claim("name", TestUser));

            SetMockOperationService();
            SetMockCategoryService();
            SetMockLocalizer();
        }

        private void SetMockLocalizer()
        {
            var localizerMock = new Mock<IStringLocalizer<SelfFinanceManagerUI.App>>();

            localizerMock.Setup(l => l[It.IsAny<string>()])
                .Returns((string key) => new LocalizedString(key, $"{key}"));

            _testContext.Services.AddSingleton(localizerMock.Object);
        }

        private void SetMockOperationService()
        {
            var operationServiceMock = new Mock<IOperationService>(); 

            SetupOperationServiceGetFinancialOperations(operationServiceMock);
            SetupOperationServiceGetOperationsByDate(operationServiceMock);
            SetupOperationServiceCreateOperation(operationServiceMock);
            SetupOperationServiceUpdateOperation(operationServiceMock);
            SetupOperationServiceDeleteOperation(operationServiceMock);

            _testContext.Services.AddSingleton<IOperationService>(operationServiceMock.Object);
        }

        private void SetMockCategoryService()
        {
            var categoryServiceMock = new Mock<ICategoryService>(); 

            SetupCategoryServiceGetCategoryById(categoryServiceMock);
            SetupCategoryServiceIsCategoryNameUnique(categoryServiceMock);
            SetupCategoryServiceCreateCategory(categoryServiceMock);
            SetupCategoryServiceGetCategories(categoryServiceMock);
            SetupCategoryServiceUpdateCategory(categoryServiceMock);
            SetupCategoryServiceDeleteCategory(categoryServiceMock);

            _testContext.Services.AddSingleton<ICategoryService>(categoryServiceMock.Object);
        }

        private void SetupCategoryServiceGetCategoryById(Mock<ICategoryService> categoryServiceMock)
        {
            categoryServiceMock
                .Setup(m => m.GetCategoryByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => _categories.FirstOrDefault(c => c.Id == id));
        }

        private void SetupCategoryServiceIsCategoryNameUnique(Mock<ICategoryService> categoryServiceMock)
        {
            categoryServiceMock
                .Setup(m => m.IsCategoryNameUniqueAsync(It.IsAny<string>()))
                .ReturnsAsync((string categoryName)
                    => !_categories.Any(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)));
        }

        private void SetupCategoryServiceCreateCategory(Mock<ICategoryService> categoryServiceMock)
        {
            categoryServiceMock
                .Setup(m => m.CreateCategoryAsync(It.IsAny<SaveCategoryDto>()))
                .ReturnsAsync((SaveCategoryDto dto) => {
                    var newCategory = new Category { Id = _categories.Max(c => c.Id) + 1, Name = dto.Name };
                    _categories.Add(newCategory);
                    return newCategory;
                });
        }

        private void SetupCategoryServiceGetCategories(Mock<ICategoryService> categoryServiceMock)
        {
            categoryServiceMock
                .Setup(m => m.GetCategoriesAsync())
                .ReturnsAsync(_categories);
        }

        private void SetupCategoryServiceUpdateCategory(Mock<ICategoryService> categoryServiceMock)
        {
            categoryServiceMock
                .Setup(m => m.UpdateCategoryAsync(It.IsAny<int>(), It.IsAny<SaveCategoryDto>()))
                .ReturnsAsync((int id, SaveCategoryDto dto) => {
                    var updatedCategory = _categories.FirstOrDefault(c => c.Id == id);
                    if (updatedCategory != null)
                    {
                        updatedCategory.Name = dto.Name;
                    }
                    return updatedCategory;
                });
        }

        private void SetupCategoryServiceDeleteCategory(Mock<ICategoryService> categoryServiceMock)
        {
            categoryServiceMock
                .Setup(m => m.DeleteCategoryAsync(It.IsAny<int>()))
                .Returns<int>(id => {
                    var categoryToRemove = _categories.FirstOrDefault(c => c.Id == id);
                    if (categoryToRemove != null)
                    {
                        _categories.Remove(categoryToRemove);
                        return Task.FromResult(categoryToRemove);
                    }
                    return Task.FromResult<Category>(null);
                });
        }

        private void SetupOperationServiceGetFinancialOperations(Mock<IOperationService> operationServiceMock)
        {
            operationServiceMock
                .Setup(m => m.GetFinancialOperationsAsync())
                .ReturnsAsync(_operations);
        }

        private void SetupOperationServiceGetOperationsByDate(Mock<IOperationService> operationServiceMock)
        {
            operationServiceMock
                .Setup(m => m.GetOperationsByDateAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new OperationsByDate
                {
                    Income = _operations.Where(o => o.IsIncome).Sum(o => o.Amount),
                    Expenses = _operations.Where(o => !o.IsIncome).Sum(o => o.Amount),
                    Operations = _operations
                });
        }

        private void SetupOperationServiceCreateOperation(Mock<IOperationService> operationServiceMock)
        {
            operationServiceMock
                .Setup(m => m.CreateOperationAsync(It.IsAny<SaveOperationDto>()))
                .ReturnsAsync((SaveOperationDto dto) => {
                    var newOperation = new FinancialOperation
                    {
                        Id = _operations.Max(o => o.Id) + 1,
                        Name = dto.Name,
                        Amount = dto.Amount,
                        Date = dto.Date,
                        IsIncome = dto.IsIncome,
                        CategoryId = dto.CategoryId
                    };
                    _operations.Add(newOperation);
                    return newOperation;
                });
        }

        private void SetupOperationServiceUpdateOperation(Mock<IOperationService> operationServiceMock)
        {
            operationServiceMock
                .Setup(m => m.UpdateOperationAsync(It.IsAny<int>(), It.IsAny<SaveOperationDto>()))
                .ReturnsAsync((int id, SaveOperationDto dto) => {
                    var operationToUpdate = _operations.FirstOrDefault(o => o.Id == id);
                    if (operationToUpdate == null)
                        return null;

                    operationToUpdate.Name = dto.Name;
                    operationToUpdate.Amount = dto.Amount;
                    operationToUpdate.Date = dto.Date;
                    operationToUpdate.IsIncome = dto.IsIncome;
                    operationToUpdate.CategoryId = dto.CategoryId;

                    return operationToUpdate;
                });
        }

        private void SetupOperationServiceDeleteOperation(Mock<IOperationService> operationServiceMock)
        {
            operationServiceMock
                .Setup(m => m.DeleteOperationAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) => {
                    var operationToDelete = _operations.FirstOrDefault(o => o.Id == id);
                    if (operationToDelete != null)
                    {
                        _operations.Remove(operationToDelete);
                    }
                    return operationToDelete;
                });
        }
    }
}