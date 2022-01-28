using IssueTracker.Contracts.V1;
using IssueTracker.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,  Roles = "SuperAdmin, Admin")]
    public class AuthorizationController : Controller
    {
        private readonly Services.IAuthorizationService auth;
        public AuthorizationController(Services.IAuthorizationService auth = null)
        {
            this.auth = auth;
        }

        [HttpPost(ApiRoutes.Authorization.AddClaimToUserAsync)]
        public async Task<IActionResult> AddClaimToUserAsync(string userEmail, string claimName, string claimValue)
        {
            var result = await auth.AddClaimToUserAsync(userEmail, claimName, claimValue);

            if (!result.Success) return BadRequest(result.Errors);

            return Ok();
        }

        [HttpPost(ApiRoutes.Authorization.AddClaimToRoleAsync)]
        public async Task<IActionResult> AddClaimToRoleAsync(string claim, string claimValue, string role)
        {
            var result = await auth.AddClaimToRoleAsync(claim, claimValue, role);

            if (!result.Success) return BadRequest(result.Errors);

            return Ok();
        }

        [HttpPost(ApiRoutes.Authorization.AddUserToRoleAsync)]
        [Authorize(Policy = "IsDotComEmail")]
        public async Task<IActionResult> AddUserToRoleAsync(string userEmail, string role)
        {
            var result = await auth.AddUserToRoleAsync(userEmail, role);

            if (!result.Success) return BadRequest(result.Errors);

            return Ok();
        }

        [HttpDelete(ApiRoutes.Authorization.RemoveClaimFromRoleAsync)]
        public async Task<IActionResult> RemoveClaimFromRoleAsync(string claim, string role)
        {
            var result = await auth.RemoveClaimFromRoleAsync(claim, role);

            if (!result.Success) return BadRequest(result.Errors);

            return Ok();
        }

        [HttpDelete(ApiRoutes.Authorization.RemoveClaimFromUserAsync)]
        public async Task<IActionResult> RemoveClaimFromUserAsync(string userEmail, string claim)
        {
            var result = await auth.RemoveClaimFromUserAsync(userEmail, claim);

            if (!result.Success) return BadRequest(result.Errors);

            return Ok();
        }

        [HttpDelete(ApiRoutes.Authorization.RemoveUserFromRoleAsync)]
        public async Task<IActionResult> RemoveUserFromRoleAsync(string userEmail, string role)
        {
            var result = await auth.RemoveUserFromRoleAsync(userEmail, role);

            if (!result.Success) return BadRequest(result.Errors);

            return Ok();
        }

    }
}
