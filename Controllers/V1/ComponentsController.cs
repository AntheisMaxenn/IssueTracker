using AutoMapper;
using IssueTracker.Contracts.V1;
using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Domain;
using IssueTracker.DTO;
using IssueTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentsController : ControllerBase
    {
        private readonly IComponentService componentService;
        private readonly ILogger<ComponentsController> logger;
        private readonly IMapper mapper;

        public ComponentsController(IComponentService componentService, ILogger<ComponentsController> logger, IMapper mapper)
        {
            this.componentService = componentService;
            this.logger = logger;
            this.mapper = mapper;
        }

        // Create
        [HttpPost(ApiRoutes.Components.Create)]
        public async Task<ActionResult> Create([FromBody] NameDescriptionRequest nameDescriptionRequest)
        {
            var result = await componentService.CreateComponent(nameDescriptionRequest);

            if (!result.Success) return BadRequest("Error: Unable to Create Component");

            return Ok(result.Success);
        }



        // GetMachine
        [HttpGet(ApiRoutes.Components.Get)]
        public async Task<ActionResult> GetComponent([FromRoute] int componentId)
        {
            var result = await componentService.GetComponent(componentId);


            if (result.Data == null) return NotFound($"Error: Unable Find Machine {componentId}.");

            return Ok(result.Data);
        }

        // UpdateMachine
        [HttpPut(ApiRoutes.Components.Update)]
        public async Task<ActionResult> UpdateComponent([FromBody] ComponentDTO componentDTO)
        {
            var result = await componentService.UpdateComponent(componentDTO.ComponentId, componentDTO);

            if (!result.Success) return NotFound($"Error: Unable to update Component {componentDTO.ComponentId}. Error: {String.Join(",", result.Errors)}.");

            return Accepted(result.Success);
        }

        // DeleteMachine
        [HttpDelete(ApiRoutes.Components.Delete)]
        public async Task<ActionResult> DeleteComponent([FromRoute] int componentId)
        {
            var result = await componentService.DeleteComponent(componentId);
            if (!result.Success) return NotFound($"Error: Unable to delete Component: {componentId}");

            return Accepted(result.Success);
        }

        // GetAllComponents
        [HttpGet(ApiRoutes.Components.GetAll)]
        public async Task<ActionResult> GetAllComponents([FromQuery] PaginationQuery query, [FromQuery] string? nameSearch)
        {
            var pagedFilter = mapper.Map<PaginationFilter>(query);

            var result = await componentService.GetAllComponent(pagedFilter, nameSearch);

            if (result.Data == null) return BadRequest("There was an Error.");

            return Ok(result.Data);
        }
    }
}
