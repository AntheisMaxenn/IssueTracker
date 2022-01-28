using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Domain;
using IssueTracker.DTO;

namespace IssueTracker.Services
{
    public interface IComponentService
    {
        Task<CommandResponse> CreateComponent(NameDescriptionRequest nameDescriptionRequest);

        Task<PagedResponse<PagedSuccessResponse<ComponentDTO>>> GetAllComponent(PaginationFilter pageFilter, string? nameFilter);

        Task<SingularResponse<ComponentDTO>> GetComponent(int componentId);

        Task<CommandResponse> UpdateComponent(int componentId, ComponentDTO updateComponent);

        Task<CommandResponse> DeleteComponent(int componentId);

    }
}
