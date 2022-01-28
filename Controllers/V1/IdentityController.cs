using IssueTracker.Contracts.V1;
using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Contracts.V1.Responses;
using IssueTracker.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers.V1
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        public readonly ILogger<IdentityController> Logger;
        public readonly IIdentityService Identity;

        // Service Injection


        public IdentityController(ILogger<IdentityController> logger, IIdentityService identity)
        {
            Logger = logger;
            Identity = identity;
        }


        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<ActionResult> Register([FromBody]EmployeeRegistrationRequest request)
        {
            var authResponse = await Identity.Register(request);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }


        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<ActionResult> Login([FromBody] EmployeeLoginRequest request)
        {
            var authResponse = await Identity.Login(request);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var authResponse = await Identity.Refresh(request);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResponse.Token,
                RefreshToken = authResponse.RefreshToken
            });
        }

    }
}
