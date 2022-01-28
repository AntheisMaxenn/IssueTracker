using IssueTracker.Contracts.V1.Requests;

namespace IssueTracker.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<bool> CreateEmployee(EmployeeRegistrationRequest employeeRegistrationRequest);

    }
}
