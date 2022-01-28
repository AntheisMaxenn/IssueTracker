using Microsoft.AspNetCore.Authorization;

namespace IssueTracker.Authorization
{
    public class IsDotComEmailRequirement : IAuthorizationRequirement
    {
        public string emailDomain;
        public IsDotComEmailRequirement(string emailDomain)
        {
            this.emailDomain = emailDomain;
        }
    }
}
