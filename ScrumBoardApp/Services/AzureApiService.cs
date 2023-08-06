using Newtonsoft.Json;
using ScrumBoardApp.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ScrumBoardApp.Services
{
    public class AzureApiService : IAzureApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string pat = "bx7ccaujethfwusz4m53beuzoatwdb6nl4csxpzw2mbzmyhkb75q";

        public AzureApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

    
        /// <summary>
        /// Get all the work items with work item query
        /// </summary>
        /// <returns></returns>
        public async Task<List<WorkItem>> GetWorkItemsAsync()
        {
            try
            {
                string apiUrl = $"https://dev.azure.com/cnr1724/TaskManagement/_apis/wit/wiql?api-version=7.0";

                var wiqlQuery = new
                {
                    query = "SELECT [System.Id], [System.Title],[System.Description], [Custom.Deadline] FROM workitems"
                };

                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));

                var json = JsonConvert.SerializeObject(wiqlQuery);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var response1 = await _httpClient.PostAsync(apiUrl, content);
                    string responseBody = await response1.Content.ReadAsStringAsync();

                    if (response1.IsSuccessStatusCode)
                    {
                        WorkItemDefs workItemDefinitions = JsonConvert.DeserializeObject<WorkItemDefs>(responseBody); ;

                        List<string> idsList = new List<string>();
                        if (workItemDefinitions != null)
                        {
                            foreach (WorkItem workItem in workItemDefinitions.WorkItems)
                            {
                                idsList.Add(workItem.Id);
                            }
                        }
                        if (idsList.Count > 0)
                        {
                            string idsStr = string.Join(",", idsList);
                            return await GetWorkItemsDetails(idsStr);
                        }
                    }
                }
                return new List<WorkItem>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<WorkItem>();
            }
        }

        /// <summary>
        /// get work item details
        /// </summary>
        /// <param name="idsStr"></param>
        /// <returns></returns>
        public async Task<List<WorkItem>> GetWorkItemsDetails(string idsStr)
        {
            try
            {
                // Prepare the API URL for Work Items Batch (API version 7.0 or the latest available version)
                string url = $"https://dev.azure.com/cnr1724/TaskManagement/_apis/wit/workitems?ids=" + idsStr + "&$expand=all&api-version=7.0";

                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));

                var response = await _httpClient.GetAsync(url);
                var responseBody = await response.Content.ReadAsStringAsync();
                string refinedResponseBody = null;
                if (responseBody != null)
                {
                    refinedResponseBody = responseBody.Replace("System.", "").Replace("Custom.", "");
                }
                Response workItemsResponse = JsonConvert.DeserializeObject<Response>(refinedResponseBody);

                return workItemsResponse.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new List<WorkItem>();
            }
        }

        /// <summary>
        /// create new work item
        /// </summary>
        /// <param name="createdNewItem"></param>
        /// <returns></returns>
        public async Task<bool> CreateWorkItemAsync(CreateWorkItem createdNewItem)
        {
            try
            {
                string requrl = "https://dev.azure.com/cnr1724/TaskManagement/_apis/wit/workitems/$Task?api-version=7.0";

                var titleData = new
                {
                    op = "add",
                    path = "/fields/System.Title",
                    value = createdNewItem.Title,
                };

                var descData = new
                {
                    op = "add",
                    path = "/fields/System.Description",
                    value = createdNewItem.Description,
                };

                var deadlineData = new
                {
                    op = "add",
                    path = "/fields/Custom.Deadline",
                    value = DateTime.Now.AddDays(7),
                };

                string workItemStr = "[" + JsonConvert.SerializeObject(titleData) + ", " + JsonConvert.SerializeObject(descData) + "," + JsonConvert.SerializeObject(deadlineData) + "]";

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));

                    using (var content = new StringContent(workItemStr, Encoding.UTF8, "application/json-patch+json"))
                    {
                        var response = await httpClient.PostAsync(requrl, content);
                        if (response.IsSuccessStatusCode)
                        {
                            return true;
                        }
                        else
                        {
                            var errorResponse = await response.Content.ReadAsStringAsync();
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching work items: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Delete work itrem
        /// </summary>
        /// <param name="workItemId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteWorkItemAsync(int workItemId)
        {
            try
            {
                string requestUri = $" https://dev.azure.com/cnr1724/TaskManagement/_apis/wit/workitemsdelete?api-version=7.1-preview.1";

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($":{pat}")));

                string workItemStr = "{\"ids\":[" + workItemId + "],\"destroy\":true,\"skipNotifications\":true}";
                using (var content = new StringContent(workItemStr, Encoding.UTF8, "application/json"))
                {
                    var response = await _httpClient.PostAsync(requestUri, content);
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching work items: {ex.Message}");
                return false;
            }
        }
    }
}
