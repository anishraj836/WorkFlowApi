namespace WorkflowApi.Models
{
    public class Action
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public List<string> FromStates { get; set; }
        public string ToState { get; set; }
    }
}