namespace IssueTracker.Domain
{
    public class PagedResponse<T>
    {
        // Errors
        public IEnumerable<string> Errors { get; set; }

        // Data<T> T = 
        public T Data { get; set; }

    }
}
