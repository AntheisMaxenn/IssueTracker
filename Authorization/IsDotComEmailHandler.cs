using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace IssueTracker.Authorization
{
    public class IsDotComEmailHandler : AuthorizationHandler<IsDotComEmailRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsDotComEmailRequirement requirement)
        {
            var userEmailAddress = context.User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            if (userEmailAddress.Contains(requirement.emailDomain))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}
