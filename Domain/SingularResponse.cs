namespace IssueTracker.Domain
{
    public class SingularResponse<T>
    {
        public IEnumerable<string>? Errors { get; set; }

        public T? Data { get; set; }

    }
}
