using Microsoft.AspNetCore.Mvc;
using Moq;
using ScrumBoardApp.Controllers;
using ScrumBoardApp.Models;
using ScrumBoardApp.Services;

namespace ScrumBoardUnitTest
{
    [TestFixture]
    public class CreateWorkItemControllerTests
    {
        private Mock<IAzureApiService> _azureApiServiceMock;
        private CreateWorkItemController _controller;

        [SetUp]
        public void Setup()
        {
            _azureApiServiceMock = new Mock<IAzureApiService>();
            _controller = new CreateWorkItemController(_azureApiServiceMock.Object);
        }

        [Test]
        public void NewItem_ReturnsView()
        {
            // Act
            var result = _controller.NewWorkItem() as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsNull(result.Model); 
        }

        [Test]
        public async Task NewItem_Post_RedirectsToWorkItemsOnSuccessfulCreation()
        {
            // Arrange
            var newItem = new CreateWorkItem {};
            _azureApiServiceMock.Setup(x => x.CreateWorkItemAsync(newItem))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.NewWorkItem(newItem) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("WorkItems", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }

        [Test]
        public async Task NewItem_Post_RedirectsToErrorOnFailedCreation()
        {
            // Arrange
            var newItem = new CreateWorkItem {};
            _azureApiServiceMock.Setup(x => x.CreateWorkItemAsync(newItem))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.NewWorkItem(newItem) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual("Error", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
        }
    }
}
