using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Data;
using IssueTracker.Domain;
using IssueTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services.Implementations
{
    public class ActionService : IActionService
    {
        private readonly IssueTrackerDbContext context;
        private readonly ILogger<ActionService> logger;
        private readonly IIssueService issueService;

        public ActionService(IssueTrackerDbContext context, ILogger<ActionService> logger, IIssueService issueService)
        {
            this.context = context;
            this.logger = logger;
            this.issueService = issueService;
        }

        public async Task<CommandResponse> CreateAction(Data.Action action)
        {
            try
            {
                context.Add(action);

                await context.SaveChangesAsync();

                return new CommandResponse{ Success = true };
            }
            catch(Exception e)
            {
                logger.LogError($"There was an error: {e}, on CreateAction");

                return new CommandResponse
                {
                    Errors = new[] { $"Cannot Create Action" }
                };
            }
        }

        //public async Task<CommandResponse> UpdateAction(Data.Action action)
        //{
        //    var actionToUpdate = await context.Actions.AsNoTracking().FirstOrDefaultAsync(x => x == action);
        //    if (actionToUpdate == null) return new CommandResponse
        //    {
        //        Errors = new[] {$"Cannot locate Action"}
        //    };

        //    context.Actions.Update(action);

        //    var results = await context.SaveChangesAsync();

        //    if(results == 0) return new CommandResponse
        //    {
        //        Errors = new[] { $"Cannot save Update" }
        //    };

        //    return new CommandResponse
        //    {
        //        Success = true
        //    };

        //}

        public async Task<CommandResponse> DeleteAction(int actionId)
        {
            // TODO make sure its not the first action.

            try
            {
                var actionToUpdate = await context.Actions.AsNoTracking().FirstOrDefaultAsync(x => x.ActionId == actionId);

                if (actionToUpdate == null) return new CommandResponse
                {
                    Errors = new[] { $"Cannot locate Action" }
                };

                context.Actions.Remove(actionToUpdate);

                var results = await context.SaveChangesAsync();

                if (results == 0) return new CommandResponse
                {
                    Errors = new[] { $"Cannot Delete Action" }
                };

                return new CommandResponse
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                logger.LogError($"DeleteAction Exception {e}");

                return new CommandResponse
                {
                    Errors = new[] { $"There was an error: {e}" }
                };
            }
        }

        public async Task<PagedResponse<PagedSuccessResponse<Data.Action>>> GetRespectiveActions(PaginationFilter paginationFilter, int issueId)
        {
            var actions = context.Actions.AsQueryable();
            var skip = (paginationFilter.PageNumber * paginationFilter.PageSize) - paginationFilter.PageSize;

            try
            {
                if (!string.IsNullOrEmpty(issueId.ToString()))
                {
                    actions = actions.Where(x => x.IssueId == issueId);
                }

                var respectiveActions = await actions.OrderBy(x => x.Issue.DateTime).ToListAsync();
                var total = await actions.CountAsync();

                //var respectiveActions = await context.Actions.OrderBy(x => x.Issue.DateTime).ToListAsync();

                PagedSuccessResponse<Data.Action> data = new PagedSuccessResponse<Data.Action>(respectiveActions);

                data.Total = total;

                return new PagedResponse<PagedSuccessResponse<Data.Action>>
                {
                    Data = data
                };

            }catch(Exception e)
            {
                logger.LogError($"GetRespectiveActions Exception {e}");

                return new PagedResponse<PagedSuccessResponse<Data.Action>>
                {
                    Errors = new[] {$"There was an error: {e}"}
                };

            }
        }

    }
}
