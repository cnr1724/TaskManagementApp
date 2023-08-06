namespace ScrumBoardApp.Models
{
    public class WorkItemFields
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
    }

    public class WorkItem
    {
        public string Id { get; set; }
        public WorkItemFields Fields { get; set; }
    }
    public class Response
    {
       public List<WorkItem> Value { get; set;}
    }

    public class WorkItemDefs
    {
        public List<WorkItem>   WorkItems { get; set; }
    }
}
