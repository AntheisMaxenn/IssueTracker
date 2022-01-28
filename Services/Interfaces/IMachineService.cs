using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Data;
using IssueTracker.Domain;

namespace IssueTracker.Services
{
    public interface IMachineService
    {

        Task<CommandResponse> CreateMachine(NameDescriptionRequest nameDescriptionRequest);

        Task<PagedResponse<PagedSuccessResponse<MachineDTO>>> GetAllMachines(PaginationFilter pageFilter, string? nameFilter);

        Task<SingularResponse<MachineDTO>> GetMachine(int machineId);

        Task<CommandResponse> UpdateMachine(int machineId, MachineDTO updateMachine);

        Task<CommandResponse> DeleteMachine(int machineId);

    }
}
