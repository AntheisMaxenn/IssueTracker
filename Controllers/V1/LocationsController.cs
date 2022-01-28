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
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService locationService;
        private readonly ILogger<LocationsController> logger;
        private readonly IMapper mapper;
        public LocationsController(ILocationService locationService, ILogger<LocationsController> logger, IMapper mapper)
        {
            this.locationService = locationService;
            this.logger = logger;
            this.mapper = mapper;
        }

        // Get
        [HttpGet(ApiRoutes.Locations.Get)]
        public async Task<ActionResult> GetLocation([FromRoute] int locationId)
        {
            var location = await locationService.GetLocation(locationId);
            
            if(location.Errors != null) return NotFound($"There was an error: {String.Join(",", location.Errors)}");
            
            return Ok(location.Data);
        }

        // GetAll + Search
        [HttpGet(ApiRoutes.Locations.GetAll)]
        public async Task<ActionResult> GetAllLocations([FromQuery] PaginationQuery paginationQuery, [FromQuery] string? nameFilter)
        {
            var paginationFilter = mapper.Map<PaginationFilter>(paginationQuery);
            var pagination = await locationService.GetAllLocation(paginationFilter, nameFilter);


            if (pagination.Data == null) return BadRequest($"There was an error. {String.Join(",", pagination.Errors)}");

            return Ok(pagination.Data);
        }

        // Create
        [HttpPost(ApiRoutes.Locations.Create)]
        public async Task<ActionResult<LocationDTO>> CreateLocation([FromBody] NameDescriptionRequest location)
        {
            var result = await locationService.CreateLocation(location);

            if (!result.Success) return BadRequest($"There was an error, {String.Join(",", result.Errors)}.");

            return Accepted(result.Success);
        }

        // Update
        [HttpPut(ApiRoutes.Locations.Update)]
        public async Task<ActionResult<LocationDTO>> UpdateLocation([FromBody] LocationDTO locationDTO)
        {
            var result = await locationService.UpdateLocation(locationDTO.LocationId, locationDTO);

            if (!result.Success) return BadRequest($"There was an error, {String.Join("," , result.Errors)}");

            return Accepted(result.Success);
        }

        // Delete
        [HttpDelete(ApiRoutes.Locations.Delete)]
        public async Task<ActionResult<LocationDTO>> DeleteLocation([FromRoute] int locationId)
        {
            var result = await locationService.DeleteLocation(locationId);

            if (!result.Success) return BadRequest($"There was an error, {String.Join(",", result.Errors)}");

            return Accepted(result.Success);

        }

    }
}
