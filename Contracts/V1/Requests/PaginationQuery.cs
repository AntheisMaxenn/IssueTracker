namespace IssueTracker.Contracts.V1.Requests
{
    public class PaginationQuery
    {
        public PaginationQuery()
        {
            PageNumber = 1;
            PageSize = 25;
        }

        public PaginationQuery(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber > 0 ? PageNumber : 1;
            PageSize = pageSize > 0 && pageSize <= 25 ? PageSize : 25;
        }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
