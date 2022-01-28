namespace IssueTracker.Domain
{
    public class CommandResponse
    {
        public bool Success { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}
