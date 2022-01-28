using AutoMapper;
using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Data;
using IssueTracker.Domain;
using IssueTracker.DTO;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services.Implementations
{
    public class ComponentService : IComponentService
    {
        private readonly IssueTrackerDbContext context;
        private readonly ILogger<ComponentService> logger;
        private readonly IMapper mapper;

        public ComponentService(IssueTrackerDbContext context, ILogger<ComponentService> logger, IMapper mapper)
        {
            this.context = context;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<CommandResponse> CreateComponent(NameDescriptionRequest nameDescriptionRequest)
        {

            try
            {
                var component = mapper.Map<Component>(nameDescriptionRequest);
                await context.Components.AddAsync(component);
                var result = await context.SaveChangesAsync();
                
                // failed
                if (result == 0) return new CommandResponse
                {
                    Errors = new[] { "Uable able to add new Component." }
                };

                // Success
                return new CommandResponse
                {
                    Success = true
                };

            }catch(Exception e)
            {

                logger.LogError($"Error creating new Component, Error: {e}");

                return new CommandResponse
                {
                    Errors = new[] { "An Error occured." }
                };

            }

        }

        public async Task<CommandResponse> DeleteComponent(int componentId)
        {

            try
            {
                var componentToDelete = await context.Components.FirstOrDefaultAsync(x => x.ComponentId == componentId);
                if(componentToDelete == null) return new CommandResponse
                {
                    Errors = new[] { "Unable to find component." }
                };

                context.Components.Remove(componentToDelete);
                var result = await context.SaveChangesAsync();

                // failed
                if (result == 0) return new CommandResponse
                {
                    Errors = new[] { "Uable able to add new Component." }
                };

                return new CommandResponse
                {
                    Success = true
                };

            }
            catch (Exception e)
            {
                logger.LogError($"Error deleteing Component, Error: {e}");

                return new CommandResponse
                {
                    Errors = new[] { "An Exception occured." }
                };

            }
        }

        public async Task<PagedResponse<PagedSuccessResponse<ComponentDTO>>> GetAllComponent(PaginationFilter pageFilter, string? nameFilter)
        {
            // Building query
            var components = context.Components.AsQueryable();
            var skip = (pageFilter.PageNumber * pageFilter.PageSize) - pageFilter.PageSize;
            if (!string.IsNullOrEmpty(nameFilter))
            {
                components = components.Where(x => x.Name.Contains(nameFilter));
            }

            try
            {
                var total = await components.CountAsync();

                var data = await components.Skip(skip).Take(pageFilter.PageSize).ToListAsync();

                var resultsDTO = mapper.Map<IEnumerable<ComponentDTO>>(data);

                PagedSuccessResponse<ComponentDTO> results = new PagedSuccessResponse<ComponentDTO>(resultsDTO);

                results.Total = total;

                return new PagedResponse<PagedSuccessResponse<ComponentDTO>>
                {
                    Data = results
                };

            }catch(Exception e)
            {

                logger.LogError($"There was an error on GetAllComponents, Error: {e}");

                return new PagedResponse<PagedSuccessResponse<ComponentDTO>>
                {
                    Errors = new[] { "An Exception occured." }
                };
            }
        }

        public async Task<SingularResponse<ComponentDTO>> GetComponent(int componentId)
        {
            try
            {
                var component = await context.Components.FirstOrDefaultAsync(x => x.ComponentId == componentId);
                var result = mapper.Map<ComponentDTO>(component);

                if (result == null) return new SingularResponse<ComponentDTO>
                {
                    Errors = new[] { $"There was an error getting Component {componentId}" }
                };

                return new SingularResponse<ComponentDTO>
                {
                    Data = result
                };

            }catch(Exception e)
            {
                logger.LogError($"There was an error getting component: {componentId}. Error: {e}");

                return new SingularResponse<ComponentDTO>
                {
                    Errors = new[] { "An Exception occured." }
                };

            }


        }

        public async Task<CommandResponse> UpdateComponent(int componentId, ComponentDTO updateComponent)
        {
            try
            {

                var component = mapper.Map<Component>(updateComponent);

                //if (!(await ComponentExists(componentId))) return false;

                var exists = await ComponentExists(updateComponent.ComponentId);

                if (!exists) return new CommandResponse
                {
                    Errors = new[] { $"Unable to get Componet {componentId}" }
                };

                context.Components.Update(component);

                await context.SaveChangesAsync();

                return new CommandResponse
                {
                    Success = true
                };

            }catch (Exception e)
            {
                logger.LogError($"There was an Exception updating a Component: {e}");
                return new CommandResponse
                {
                    Errors = new[] { "An Exception occured." }
                };
            }
        }


        private async Task<bool> ComponentExists(int componentId)
        {
            return await context.Components.AsNoTracking().AnyAsync(x => x.ComponentId == componentId);
        }
    }
}
