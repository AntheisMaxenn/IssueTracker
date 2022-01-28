using AutoMapper;
using IssueTracker.Cache;
using IssueTracker.Contracts.V1;
using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Data;
using IssueTracker.Domain;
using IssueTracker.Filters;
using IssueTracker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers.V1
{
    //[Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MachinesController : ControllerBase
    {
        private readonly IMachineService machineService;
        private readonly ILogger<MachinesController> logger;
        private readonly IMapper mapper;


        public MachinesController(IMachineService machineService, ILogger<MachinesController> logger, IMapper mapper)
        {
            this.machineService = machineService;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpPost(ApiRoutes.Machines.Create)]
        [TypeFilter(typeof(ActionFilterAttribute), Arguments = new object[] { ApiRoutes.Machines.Create })]
        public async Task<ActionResult> CreateMachine([FromBody] NameDescriptionRequest nameDescriptionRequest)
        {
            var result = await machineService.CreateMachine(nameDescriptionRequest);

            if (!result.Success) return BadRequest("Error: Unable to Create Machine");


            return Ok(result.Success);

        }

        [HttpGet(ApiRoutes.Machines.GetAll)]
        [Cached(60)]
        public async Task<ActionResult<PagedSuccessResponse<MachineDTO>>> GetAllMachines([FromQuery] PaginationQuery query, [FromQuery] string? nameSearch)
        {
            var pagedFilter = mapper.Map<PaginationFilter>(query);

            var result = await machineService.GetAllMachines(pagedFilter, nameSearch);

            if (result == null) return BadRequest("There was an Error.");

            return Ok(result.Data);

        }

        // Get Single Machine By Id

        [HttpGet(ApiRoutes.Machines.Get)]
        public async Task<ActionResult> GetMachine([FromRoute]int machineId)
        {
            var result = await machineService.GetMachine(machineId);

            if (result.Errors != null) return NotFound($"There was an Error: {string.Join("," , result.Errors)}");

            return Ok(result.Data);
        }
        

        // Update

        [HttpPut(ApiRoutes.Machines.Update)]
        public async Task<ActionResult> UpdateMachine([FromBody] MachineDTO updateMachine)
        {

                var result = await machineService.UpdateMachine(updateMachine.MachineId, updateMachine);

                if (!result.Success) return BadRequest($"Error: {string.Join("," , result.Errors)}");

                return Accepted(result.Success);

        }

        // Delete
        [HttpDelete(ApiRoutes.Machines.Delete)]
        public async Task<ActionResult> DeleteMachine(int machineId)
        {
            var result = await machineService.DeleteMachine(machineId);
            if (!result.Success) return NotFound($"Error: Unable to delete Machine, Error: {string.Join("," , result.Errors)}");

            return Accepted(result.Success);

        }



    }
}
