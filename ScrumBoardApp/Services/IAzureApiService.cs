using ScrumBoardApp.Models;

namespace ScrumBoardApp.Services
{
    public interface IAzureApiService
    {
        public Task<List<WorkItem>> GetWorkItemsAsync();
        public Task<List<WorkItem>> GetWorkItemsDetails(string idsStr);
        public Task<bool> CreateWorkItemAsync(CreateWorkItem createdNewItem);
        public Task<bool> DeleteWorkItemAsync(int workItemId);
    }
}
