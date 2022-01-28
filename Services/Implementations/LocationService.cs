using AutoMapper;
using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Data;
using IssueTracker.Domain;
using IssueTracker.DTO;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services.Implementations
{
    public class LocationService : ILocationService
    {
        private readonly IssueTrackerDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<LocationService> logger;
        public LocationService(IssueTrackerDbContext context, IMapper mapper, ILogger<LocationService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }
        public async Task<CommandResponse> CreateLocation(NameDescriptionRequest nameDescriptionRequest)
        {

            try
            {
                var location = mapper.Map<Location>(nameDescriptionRequest);

                await context.Locations.AddAsync(location);

                var result = await context.SaveChangesAsync();

                return new CommandResponse
                {
                    Success = true
                };

            }catch (Exception e)
            {

                logger.LogError($"Exception: Create Location Exception: {e}");

                return new CommandResponse
                {
                    Errors = new[] { $"There was an exception." }
                };
            }
        }

        public async Task<CommandResponse> DeleteLocation(int locationId)
        {
            try
            {

                var locationToDelete = await context.Locations.FirstOrDefaultAsync(x => x.LocationId == locationId);

                if (locationToDelete == null) return new CommandResponse
                {
                    Errors = new[] {$"Cannot find location: {locationId}"}
                };


                context.Locations.Remove(locationToDelete);

                await context.SaveChangesAsync();

                return new CommandResponse
                {
                    Success = true
                };

            }
            catch (Exception e)
            {

                logger.LogError($"There was an Exception Deleting Location {locationId}, Exception {e}");
                return new CommandResponse
                {
                    Errors = new[] { $"There was an Exception." }
                };
            }
        }

        public async Task<PagedResponse<PagedSuccessResponse<LocationDTO>>> GetAllLocation(PaginationFilter pageFilter, string? nameFilter)
        {
            try
            {
                var locations = context.Locations.AsQueryable();
                var skip = (pageFilter.PageNumber * pageFilter.PageSize) - pageFilter.PageSize;


                if (!string.IsNullOrEmpty(nameFilter))
                {
                    locations = locations.Where(x => x.Name.Contains(nameFilter));
                }

                var total = await locations.CountAsync();

                var data = await locations.Skip(skip).Take(pageFilter.PageSize).ToListAsync();

                var resultsDTO = mapper.Map<IEnumerable<LocationDTO>>(data);

                PagedSuccessResponse<LocationDTO> results = new PagedSuccessResponse<LocationDTO>(resultsDTO);

                results.Total = total;

                return new PagedResponse<PagedSuccessResponse<LocationDTO>>
                {
                    Data = results
                };
            }
            catch (Exception e)
            {
                logger.LogError($"There was an Exception Get all Locations, Exception: {e}");

                return new PagedResponse<PagedSuccessResponse<LocationDTO>>
                {
                    Errors = new[] { $"There was an Exception." }
                };
            }
        }

        public async Task<SingularResponse<LocationDTO>> GetLocation(int locationId)
        {
            try
            {
                var result = await context.Locations.AsNoTracking().FirstOrDefaultAsync(x => x.LocationId == locationId);
                if (result == null) return new SingularResponse<LocationDTO>
                {
                    Errors = new[] { $"Unable to find Location: {locationId}" }
                };
                var resultDTO = mapper.Map<LocationDTO>(result);

                return new SingularResponse<LocationDTO>
                {
                    Data = resultDTO
                };
            }
            catch (Exception e)
            {
                logger.LogError($"There was an Exception, Get location: {locationId}, Exception: {e}");
                return new SingularResponse<LocationDTO>
                {
                    Errors = new[] { $"There was an Exception." }
                };
            }
        }
        public async Task<CommandResponse> UpdateLocation(int locationId, LocationDTO updateLocation)
        {
            try
            {
                var location = mapper.Map<Location>(updateLocation);

                if (!(await LocationExists(locationId))) return new CommandResponse
                {
                    Errors = new[] {$"Cant find that Location: {locationId}"}
                };

                context.Locations.Update(location);

                await context.SaveChangesAsync();

                return new CommandResponse
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                logger.LogError($"There was an Exception Updating Location: {locationId}, Exception: {e}");
                return new CommandResponse
                {
                    Errors = new[] { $"There was an Exception." }
                };
            }
        }



        private async Task<bool> LocationExists(int locationId)
        {
            return await context.Locations.AsNoTracking().AnyAsync(x => x.LocationId == locationId);
        }
    }
}
