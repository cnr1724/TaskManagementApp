using Microsoft.AspNetCore.Mvc;
using Moq;
using ScrumBoardApp.Controllers;
using ScrumBoardApp.Models;
using ScrumBoardApp.Services;

namespace ScrumBoardUnitTest
{
    public class Tests
    {
        private Mock<IAzureApiService> _azureApiServiceMock;
        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            _azureApiServiceMock = new Mock<IAzureApiService>();
            _controller = new HomeController(_azureApiServiceMock.Object);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public async Task WorkItems_ReturnsViewWithWorkItems()
        {
            try
            {
                // Arrange
                var expectedWorkItems = new List<WorkItem>
        {
            new WorkItem
            {
                Id = "12",
                Fields = new WorkItemFields
                {
                    Title = "Add",
                    Description = "Create story for add task",
                    Deadline = DateTime.Now.AddDays(1)
                }
            },
            new WorkItem
            {
                Id = "11",
                Fields = new WorkItemFields
                {
                    Title = "test",
                    Description = "test",
                    Deadline = DateTime.Now.AddDays(2)
                }
            }
        };
                _azureApiServiceMock.Setup(x => x.GetWorkItemsAsync())
                    .ReturnsAsync(expectedWorkItems);

                // Act
                var result = await _controller.WorkItems() as ViewResult;

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(expectedWorkItems, result.Model);
                Assert.AreEqual("WorkItems", result.ViewName);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                Console.WriteLine($"Exception type: {ex.GetType().FullName}");
                throw;
            }
        }

        [Test]
        public async Task DeleteTaskItem_ReturnsRedirectOnSuccessfulDeletion()
        {
            // Arrange
            var id = 1;
            _azureApiServiceMock.Setup(x => x.DeleteWorkItemAsync(id))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTaskItem(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("WorkItems", result.ActionName); 
            Assert.AreEqual("Home", result.ControllerName); 
        }

        [Test]
        public async Task DeleteTaskItem_ReturnsNotFoundOnFailure()
        {
            // Arrange
            var id = 1;
            _azureApiServiceMock.Setup(x => x.DeleteWorkItemAsync(id))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTaskItem(id) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
        }
    }
}