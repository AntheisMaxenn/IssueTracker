using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Domain;

namespace IssueTracker.Services.Interfaces
{
    public interface IActionService
    {
        Task<PagedResponse<PagedSuccessResponse<Data.Action>>> GetRespectiveActions(PaginationFilter paginationFilter, int issueId);

        Task<CommandResponse> CreateAction(ActionRequest actionRequest);

        //Task<CommandResponse> UpdateAction(Data.Action action);

        Task<CommandResponse> DeleteAction(int actionId);
    }
}
