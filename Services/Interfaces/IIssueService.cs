using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Data;
using IssueTracker.Domain;

namespace IssueTracker.Services.Interfaces
{
    public interface IIssueService
    {
        // Create Issue + Action
        Task<CommandResponse> CreateIssueAction(IssueRequest issueRequest);

        // GetAll + Search
        Task<PagedResponse<PagedSuccessResponse<Issue>>> GetAllIssue(PaginationFilter paginationFilter, string? description);

        Task<CommandResponse> UpdateIssue(IssueRequest issueRequest);

        Task<CommandResponse> DeleteIssue(int issueId);

        Task<SingularResponse<Issue>> GetIssue(int issueId);

        Task<SingularResponse<Issue>> GetDetailedIssue(int issueId);

    }
}
