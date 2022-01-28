using AutoMapper;
using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Data;
using IssueTracker.Domain;
using IssueTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services.Implementations
{
    public class IssueService : IIssueService
    {
        private readonly IssueTrackerDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<IssueService> logger;

        // Controller will call IssueExists() its self, if required.

        public IssueService(IssueTrackerDbContext context, IMapper mapper, ILogger<IssueService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<CommandResponse> CreateIssueAction(IssueRequest newIssueRequest)
        {
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try 
                {
                    var employee = await context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == newIssueRequest.EmployeeId);

                    if (employee == null) return new CommandResponse
                    {
                        Errors = new[] { $"Cannont find that Employee" }
                    };

                    await context.Issues.AddAsync(new Issue
                    {
                        Description = newIssueRequest.Description,
                        Impact = newIssueRequest.Impact,
                        Danger = newIssueRequest.Danger,
                        Classification = newIssueRequest.Classification,
                        Status = newIssueRequest.Status,
                        DateTime = DateTime.UtcNow,
                        MachineId = newIssueRequest.MachineId,
                        ComponentId = newIssueRequest.ComponentId,
                        LocationId = newIssueRequest.LocationId,

                        Actions = new List<Data.Action>
                        {
                            new Data.Action 
                            {   
                                Involvement = newIssueRequest.Involvement,
                                DateTime = DateTime.UtcNow,
                                Employee = employee
                            }
                        }
                    }
                    );

                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    return new CommandResponse
                    {
                        Success = true
                    };
                }
                catch (Exception e)
                {
                    logger.LogError($"Create new Issue Exception: {e}");

                    return new CommandResponse
                    {
                        Errors = new[] { $"There was an Exception." }
                    };
                }
            }
        }

        public async Task<PagedResponse<PagedSuccessResponse<Issue>>> GetAllIssue(PaginationFilter paginationFilter, string? description)
        {
            // Defaulting will be order by DateTime

            var skip = (paginationFilter.PageNumber * paginationFilter.PageSize) - paginationFilter.PageSize;


            try
            {
                var issues = context.Issues.AsQueryable();

                if (!string.IsNullOrEmpty(description))
                {
                    issues = issues.Where(x => x.Description.Contains(description));
                }

                var total = await issues.CountAsync();

                var returnedIssues = await issues.Skip(skip).Take(paginationFilter.PageSize).OrderBy(x => x.DateTime).ToListAsync();

                //var resultsDTO = mapper.Map<IEnumerable<MachineDTO>>(data);

                PagedSuccessResponse<Issue> data = new PagedSuccessResponse<Issue>(returnedIssues);

                data.Total = total;

                PagedResponse<PagedSuccessResponse<Issue>> results = new PagedResponse<PagedSuccessResponse<Issue>>
                {
                    Data = data
                };

                return results;

            } catch(Exception e)
            {
                logger.LogError($"GetAllIssue Exception: {e}");
                return new PagedResponse<PagedSuccessResponse<Issue>>
                {
                    Errors = new[] {$"There were errors: {e}"}
                };
            }

        }

        public async Task<CommandResponse> UpdateIssue(IssueRequest issueRequest)
        {

            try
            {
                var issue = mapper.Map<Issue>(issueRequest);

                if (issueRequest.IssueId == null) return new CommandResponse
                {
                    Errors = new[] { $"Error: issueId cannot be null." }
                };

                issue.IssueId = (int)issueRequest.IssueId;

                context.Issues.Update(issue);

                await context.SaveChangesAsync();

                return new CommandResponse
                {
                    Success = true
                };

            }catch(Exception e)
            {
                logger.LogError($"GetIssue Exception: {e}");
                return new CommandResponse
                {
                    Errors = new[] { $"There was an exception" }
                };
            }

        }

        public async Task<CommandResponse> DeleteIssue(int issueId)
        {

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var issue = context.Issues.FirstOrDefault(x => x.IssueId == issueId);
                    context.Issues.Remove(issue);

                    var actions = context.Actions.Where(x => x.IssueId == issueId);
                    context.Actions.RemoveRange(actions);

                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new CommandResponse
                    {
                        Success = true
                    };

                }
                catch (Exception e)
                {
                    logger.LogError($"DeleteIssue Exception: {e}");
                    return new CommandResponse
                    {
                        Errors = new[] { $"There was an exception" }
                    };
                }
            }
        }

        public async Task<SingularResponse<Issue>> GetIssue(int issueId)
        {

            try
            {
                var result = await context.Issues.FirstOrDefaultAsync(x => x.IssueId == issueId);

                if (result == null) return new SingularResponse<Issue>
                {
                    Errors = new[] { $"Unable to get Issue: {issueId}" }
                };

                return new SingularResponse<Issue>
                {
                    Data = result
                };
            }
            catch (Exception e)
            {

                logger.LogError($"GetIssue Exception: {e}");
                return new SingularResponse<Issue>
                {
                    Errors = new[] { $"There was an exception" }
                };
            }
        }

        public async Task<SingularResponse<Issue>> GetDetailedIssue(int issueId)
        {
            try
            {
                var issue = await context.Issues
                        //.Where(x => x.IssueId == issueId)
                        .Include(x => x.Actions)
                        .Include(x => x.Machine)
                        .Include(x => x.Component)
                        .Include(x => x.Location)
                        .FirstOrDefaultAsync(x => x.IssueId == issueId);

                if (issue == null) return new SingularResponse<Issue>
                {
                    Errors = new[] { $"Error getting detailed issue {issueId} " }
                };

                return new SingularResponse<Issue>
                {
                    Data = issue
                };
            }
            catch (Exception e)
            {
                logger.LogError($"GetDetailedIssue Exception: {e}");
                return new SingularResponse<Issue>
                {
                    Errors = new[] { $"There was an exception" }
                };
            }

        }

        private async Task<bool> IssueExists(int issueId)
        {
            return await context.Issues.AsNoTracking().AnyAsync(x => x.IssueId == issueId);
        }


    }
}
