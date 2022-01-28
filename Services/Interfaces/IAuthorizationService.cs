using IssueTracker.Domain;
using Microsoft.AspNetCore.Identity;

namespace IssueTracker.Services
{
    public interface IAuthorizationService
    {

        Task<AuthorizationResult> RemoveUserFromRoleAsync(string userEmail, string role);
        Task<AuthorizationResult> RemoveClaimFromUserAsync(string userEmail, string claim);

        Task<AuthorizationResult> AddClaimToUserAsync(string userEmail, string claim, string claimValue);
        Task<AuthorizationResult> AddUserToRoleAsync(string userEmail, string role);

        Task<AuthorizationResult> AddClaimToRoleAsync(string claim, string claimValue, string role);
        Task<AuthorizationResult> RemoveClaimFromRoleAsync(string claim, string role); 

    }
}
