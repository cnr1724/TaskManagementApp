using Microsoft.AspNetCore.Mvc;
using ScrumBoardApp.Models;
using ScrumBoardApp.Services;
using System.Diagnostics;

namespace ScrumBoardApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAzureApiService _azureApiService;
        public HomeController(IAzureApiService azureApiService)
        {
            _azureApiService = azureApiService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> WorkItems()
        {

            var workItems = await _azureApiService.GetWorkItemsAsync();

            return View("WorkItems",workItems);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteTaskItem(int id)
        {
            var result = await _azureApiService.DeleteWorkItemAsync(id);
            if (result)
            {
                return RedirectToAction("WorkItems", "Home");
            }
            else
            {
                return NotFound();
            }
        }
 
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}