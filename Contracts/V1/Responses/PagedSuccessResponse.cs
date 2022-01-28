namespace IssueTracker.Contracts.V1.Responses
{
    public class PagedSuccessResponse<T>
    {
        public PagedSuccessResponse()
        {

        }

        public PagedSuccessResponse(IEnumerable<T> Data)
        {
            this.Data = Data;
        }

        public IEnumerable<T> Data { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int Total { get; set; }
    }
}
