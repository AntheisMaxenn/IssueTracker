using IssueTracker.Domain;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IssueTracker.Services
{
    public class AuthorizationService : IAuthorizationService
    {

        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AuthorizationService(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        public async Task<AuthorizationResult> AddClaimToRoleAsync(string claim, string claimValue, string role)
        {
            var reqRole = await roleManager.FindByNameAsync(role);

            if (reqRole == null)
            {
                return new AuthorizationResult
                {
                    Errors = new[] { "Cannot Find Role" }
                };
            }

            try
            {
                var newClaim = new Claim(claim, claimValue);
                await roleManager.AddClaimAsync(reqRole, newClaim);
                return new AuthorizationResult
                {
                    Success = true
                };

            }catch
            {
                return new AuthorizationResult
                {
                    Errors = new[] { "There was an error." }
                }; ;
            }

        }

        public async Task<AuthorizationResult> AddClaimToUserAsync(string userEmail, string claim, string claimValue)
        {
            var user = await userManager.FindByEmailAsync(userEmail);

            if(user != null)
            {
                var newClaim = new Claim(claim, claimValue);

                var claimAdded = await userManager.AddClaimAsync(user, newClaim);

                if (!claimAdded.Succeeded)
                {
                    return new AuthorizationResult
                    {
                        Errors = claimAdded.Errors.Select(x => x.Description)
                    };
                }

                return new AuthorizationResult
                {
                    Success = true
                };   
            }

            return new AuthorizationResult
            {
                Errors = new[] { "Cannot Find User" }
            };

        }

        public async Task<AuthorizationResult> AddUserToRoleAsync(string userEmail, string role)
        {
            var user = await userManager.FindByEmailAsync(userEmail);

            if (user == null) return new AuthorizationResult { Errors = new[] { "Cannot find that user" } };

            // TODO Check Role exists
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (!roleExist) return new AuthorizationResult { Errors = new[] { $"The role {role} cannot be found" } };


            var result = await userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                return new AuthorizationResult
                {
                    Errors = result.Errors.Select(x => x.Description)
                };
            }

            return new AuthorizationResult
            {
                Success = true
            };


        }
        

        public async Task<AuthorizationResult> RemoveClaimFromRoleAsync(string claim, string role)
        {
            var reqRole = await roleManager.FindByNameAsync(role);
            if (reqRole == null) return new AuthorizationResult { Errors = new[] { "There was a problem finding that roll." } };

            var roleClaims = await roleManager.GetClaimsAsync(reqRole);
            var claimToRemove = roleClaims.Where(x => x.Type == claim).FirstOrDefault();
            if(claimToRemove == null) return new AuthorizationResult { Errors = new[] { "There was a problem finding that claim." } };

            var result = await roleManager.RemoveClaimAsync(reqRole, claimToRemove);

            if(!result.Succeeded) return new AuthorizationResult { Errors = new[] { $"There was an error trying to remove {claim} from {role} " } };

            return new AuthorizationResult { Success = true };
        }

        public async Task<AuthorizationResult> RemoveClaimFromUserAsync(string userEmail, string claim)
        {
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null) return new AuthorizationResult { Errors = new[] { "Could not find user" } };

            var userClaims = await userManager.GetClaimsAsync(user);
            var claimToRemove = userClaims.Where(x => x.Type == claim).FirstOrDefault();
            if (claimToRemove == null) return new AuthorizationResult { Errors = new[] { $"Cant find claim: {claim}" } };

            var result = await userManager.RemoveClaimAsync(user, claimToRemove);

            if (!result.Succeeded) return new AuthorizationResult { Errors = new[] {$"There was an error removing {claim} from {userEmail}"} };

            return new AuthorizationResult { Success = true };
        }

        public async Task<AuthorizationResult> RemoveUserFromRoleAsync(string userEmail, string role)
        {
            var user = await userManager.FindByEmailAsync(userEmail);
            if (user == null) return new AuthorizationResult { Errors = new[] { $"Could not find user {userEmail}" } };

            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists) return new AuthorizationResult { Errors = new[] { "There was a problem finding that role." } };


            var result = await userManager.RemoveFromRoleAsync(user, role);

            if(!result.Succeeded) return new AuthorizationResult { Errors = new[] { $"There was an error removing {role} from {userEmail}" } };

            return new AuthorizationResult { Success = true };
        }
    }
}
