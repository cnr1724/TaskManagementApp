using Microsoft.AspNetCore.Mvc;
using ScrumBoardApp.Models;
using ScrumBoardApp.Services;

namespace ScrumBoardApp.Controllers
{
    public class CreateWorkItemController : Controller
    {
        private readonly IAzureApiService _azureApiService;
        public CreateWorkItemController(IAzureApiService azureApiService)
        {
            _azureApiService = azureApiService;
        }
        public IActionResult NewWorkItem()
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewWorkItem(CreateWorkItem createdNewItem)
        {
            bool isCreated = await _azureApiService.CreateWorkItemAsync(createdNewItem);
            return isCreated ? RedirectToAction("WorkItems", "Home") : RedirectToAction("Error", "Home");
        }
    }
}
