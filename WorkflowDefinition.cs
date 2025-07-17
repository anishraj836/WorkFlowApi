namespace WorkflowApi.Models
{
    public class WorkflowDefinition
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<State> States { get; set; }
        public List<Action> Actions { get; set; }
    }
} 