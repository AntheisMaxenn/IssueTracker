namespace IssueTracker.Domain
{
    public class AuthorizationResult
    {
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
