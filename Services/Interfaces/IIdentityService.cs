using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Domain;
using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> Register(EmployeeRegistrationRequest registerUserRequest);
        Task<AuthenticationResult> Login(EmployeeLoginRequest employeeLoginRequest);

        Task<AuthenticationResult> Refresh(RefreshTokenRequest refreshTokenRequest);
    }
}
