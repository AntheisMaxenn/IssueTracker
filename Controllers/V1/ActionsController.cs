using AutoMapper;
using IssueTracker.Contracts.V1;
using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Domain;
using IssueTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionsController : ControllerBase
    {
        private readonly IActionService service;
        private readonly IMapper mapper;
        private readonly ILogger<ActionsController> logger;


        public ActionsController(IActionService service, IMapper mapper, ILogger<ActionsController> logger)
        {
            this.service = service;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpPost(ApiRoutes.Action.Create)]
        public async Task<ActionResult> CreateAction([FromBody]ActionRequest actionRequest)
        {
            var action = mapper.Map<Data.Action>(actionRequest);
            var result = await service.CreateAction(action);
            if (!result.Success) return BadRequest($"Unable to create Action.");

            return Accepted();
        }

        // GetRespectiveActions
        [HttpGet(ApiRoutes.Action.GetRespectiveActions)]
        public async Task<ActionResult> GetRespectiveActions([FromQuery]PaginationQuery paginationQuery, [FromRoute]int issueId)
        {
            var filter = mapper.Map<PaginationFilter>(paginationQuery);
            var result = await service.GetRespectiveActions(filter, issueId);

            if(result.Errors != null) return BadRequest(string.Join(",", result.Errors));

            return Ok(result.Data);
        }

        //  UpdateAction
        //[HttpPut(ApiRoutes.Action.Update)]
        //public async Task<ActionResult> UpdateAction([FromBody] ActionRequest actionRequest)
        //{
        //    var action = mapper.Map<Data.Action>(actionRequest);

        //    var result = await service.UpdateAction(action);
        //    if (!result.Success) return BadRequest(result.Errors);

        //    return Accepted();
        //}

        // DeleteAction
        [HttpDelete(ApiRoutes.Action.Delete)]
        public async Task<ActionResult> DeleteAction([FromRoute] int actionId)
        {

            var result = await service.DeleteAction(actionId);

            if (!result.Success) return BadRequest($"{string.Join(",", result.Errors)}");

            return Accepted();
        }
    }
}
