using AutoMapper;
using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Data;
using IssueTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services
{
    public class MachineService : IMachineService
    {
        private readonly IssueTrackerDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<MachineService> logger;
        public MachineService(IssueTrackerDbContext context, IMapper mapper, ILogger<MachineService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }
        public async Task<CommandResponse> CreateMachine(NameDescriptionRequest nameDescriptionRequest)
        {
            try
            {
                var machine = mapper.Map<Machine>(nameDescriptionRequest);

                await context.Machines.AddAsync(machine);
                await context.SaveChangesAsync();


                return new CommandResponse
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                logger.LogError($"Exception on Create Machine, Exception: {e} ");
                return new CommandResponse
                {
                    Errors = new[] { $"There was an Exception." }
                };
            }
        }

        public async Task<PagedResponse<PagedSuccessResponse<MachineDTO>>> GetAllMachines(PaginationFilter pageFilter, string? nameFilter)
        {

            try
            {
                var machines = context.Machines.AsQueryable();
                var skip = (pageFilter.PageNumber * pageFilter.PageSize) - pageFilter.PageSize;

                if (!string.IsNullOrEmpty(nameFilter))
                {
                    machines = machines.Where(x => x.Name.Contains(nameFilter));
                }

                var data = await machines.Skip(skip).Take(pageFilter.PageSize).ToListAsync();
                var total = await machines.CountAsync();

                var resultsDTO = mapper.Map<IEnumerable<MachineDTO>>(data);

                PagedSuccessResponse<MachineDTO> results = new PagedSuccessResponse<MachineDTO>(resultsDTO);

                results.Total = total;

                return new PagedResponse<PagedSuccessResponse<MachineDTO>>
                {
                    Data = results
                };
            }
            catch (Exception e)
            {

                logger.LogError($"Exception on Create Machine, Exception: {e} ");
                return new PagedResponse<PagedSuccessResponse<MachineDTO>>
                {
                    Errors = new[] { $"There was an Exception." }
                };
            }

        }

        public async Task<CommandResponse> DeleteMachine(int machineId)
        {

            try
            {
                var machineToDelete = await context.Machines.FirstOrDefaultAsync(x => x.MachineId == machineId);
                if (machineToDelete == null) return new CommandResponse
                {
                    Errors = new[] { $"Error finding Machine: {machineId}." }
                };

                context.Machines.Remove(machineToDelete);

                await context.SaveChangesAsync();

                return new CommandResponse
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                logger.LogError($"Exception on Deleting Machine: {machineId}, Exception: {e} ");
                return new CommandResponse
                {
                    Errors = new[] { $"There was an Exception." }
                };
            }

        }
        public async Task<SingularResponse<MachineDTO>> GetMachine(int machineId)
        {
            try
            {
                var result = await context.Machines.FirstOrDefaultAsync(x => x.MachineId == machineId);
                if (result == null) return new SingularResponse<MachineDTO>
                {
                    Errors = new[] { $"Unable to get Mahcine: {machineId}" }
                };
                var resultDTO = mapper.Map<MachineDTO>(result);

                return new SingularResponse<MachineDTO>
                {
                    Data = resultDTO
                };
            }
            catch (Exception e)
            {

                logger.LogError($"Exception getting Machine: {machineId}, Exception: {e} ");
                return new SingularResponse<MachineDTO>
                {
                    Errors = new[] { $"There was an Exception." }
                };
            }

        }

        public async Task<CommandResponse> UpdateMachine(int machineId, MachineDTO updatedMachine)
        {

            try
            {
                if (!(await MachineExists(updatedMachine.MachineId))) return new CommandResponse
                {
                    Errors = new[] { $"Cant find Machine: {machineId}" }
                };

                var machine = mapper.Map<Machine>(updatedMachine);
                context.Machines.Update(machine);

                await context.SaveChangesAsync();

                return new CommandResponse
                {
                    Success = true
                };
            }
            catch (Exception e)
            {

                logger.LogError($"Exception on Create Machine, Exception: {e} ");
                return new CommandResponse
                {
                    Errors = new[] { $"There was an Exception." }
                };
            }


        }

        private async Task<bool> MachineExists(int machineId)
        {
            return await context.Machines.AsNoTracking().AnyAsync(x => x.MachineId == machineId);
        }
    }
}
