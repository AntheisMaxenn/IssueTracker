namespace IssueTracker.Contracts.V1.Requests
{
    public class NameDescriptionRequest
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
