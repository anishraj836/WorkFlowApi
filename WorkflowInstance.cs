namespace WorkflowApi.Models
{
    public class WorkflowInstance
    {
        public string Id { get; set; }
        public string DefinitionId { get; set; }
        public string CurrentState { get; set; }
        public List<InstanceHistory> History { get; set; } = new();
    }

    public class InstanceHistory
    {
        public string ActionId { get; set; }
        public string FromState { get; set; }
        public string ToState { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 