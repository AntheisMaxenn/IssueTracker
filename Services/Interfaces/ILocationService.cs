using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Domain;
using IssueTracker.DTO;

namespace IssueTracker.Services
{
    public interface ILocationService
    {
        Task<CommandResponse> CreateLocation(NameDescriptionRequest nameDescriptionRequest);

        Task<PagedResponse<PagedSuccessResponse<LocationDTO>>> GetAllLocation(PaginationFilter pageFilter, string? nameFilter);

        Task<SingularResponse<LocationDTO>> GetLocation(int locationId);

        Task<CommandResponse> UpdateLocation(int locationId, LocationDTO updateLocation);

        Task<CommandResponse> DeleteLocation(int locationId);
    }
}
