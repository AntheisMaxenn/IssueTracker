using AutoMapper;
using IssueTracker.Contracts.V1;
using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Data;
using IssueTracker.Domain;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers.V1
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class IssuesController : ControllerBase
    {
        private readonly IIssueService issue;
        private readonly ILogger<IssuesController> logger;
        private readonly IMapper mapper;

        public IssuesController(IIssueService issue, ILogger<IssuesController> logger, IMapper mapper)
        {
            this.issue = issue;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpPost(ApiRoutes.Issue.CreateIssueAction)]
        public async Task<ActionResult> CreateNewIssue(IssueRequest newIssueRequest)
        {
            var identity = User.Claims.FirstOrDefault(x => x.Type == "id");
            if (identity.Value == null) return BadRequest($"There was an Error.");
            newIssueRequest.EmployeeId = identity.Value;
            var result = await issue.CreateIssueAction(newIssueRequest);

            if (!result.Success) return BadRequest("Create New Issue Failed.");

            return Accepted(result.Success);
        }

        [HttpGet(ApiRoutes.Issue.GetAll)]
        public async Task<ActionResult<PagedSuccessResponse<Issue>>> GetAll([FromQuery] PaginationQuery pagination, [FromQuery] string? description)
        {
            var filter = mapper.Map<PaginationFilter>(pagination);

            var results = await issue.GetAllIssue(filter, description);

            if (results.Equals(null)) return BadRequest("There was an error");

            return Ok(results.Data);
        }

        [HttpGet(ApiRoutes.Issue.GetDetailedById)]
        public async Task<ActionResult<PagedSuccessResponse<Issue>>> GetDetailed(int issueId)
        {

            var results = await issue.GetDetailedIssue(issueId);

            if (results.Errors != null) return BadRequest($"There was an error {results.Errors.FirstOrDefault()}");

            return Ok(results.Data);
        }

        [HttpGet(ApiRoutes.Issue.GetById)]
        public async Task<ActionResult> GetById([FromRoute] int issueId)
        {
            var result = await issue.GetIssue(issueId);

            if (result.Errors != null) return NotFound($"{string.Join("," , result.Errors)}");

            return Ok(result.Data);
        }

        [HttpPut(ApiRoutes.Issue.Update)]
        public async Task<ActionResult> Update([FromBody] IssueRequest updatedIssue)
        {
            var result = await issue.UpdateIssue(updatedIssue);

            if (!result.Success) return BadRequest($"{string.Join(",", result.Errors)}");

            return Accepted(result.Success);
        }

        [HttpDelete(ApiRoutes.Issue.Delete)]
        public async Task<ActionResult> Delete([FromRoute] int issueId)
        {
            var result = await issue.DeleteIssue(issueId);

            if (!result.Success) return NotFound($"{string.Join(",", result.Errors)}");

            return Accepted(result.Success);
        }
    }
}
