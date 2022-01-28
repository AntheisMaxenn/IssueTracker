using AutoMapper;
using IssueTracker.Contracts.V1.Requests;
using IssueTracker.Data;
using IssueTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IssueTrackerDbContext context;
        private readonly IMapper mapper;
        private readonly ILogger<EmployeeService> logger;


        public EmployeeService(IssueTrackerDbContext context, IMapper mapper, ILogger<EmployeeService> logger)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<bool> CreateEmployee(EmployeeRegistrationRequest employeeRegistrationRequest)
        {
            var existsAlready = await EmployeeExists(employeeRegistrationRequest.Email);

            if(!existsAlready) return false;

            try
            {
                var employee = mapper.Map<Employee>(employeeRegistrationRequest);
                await context.Employees.AddAsync(employee);
                return true;

            }catch(Exception ex)
            {
                logger.LogError($"Error mapping or creating Employee: {employeeRegistrationRequest}");
                return false;
            }

        }

        public async Task<bool> EmployeeExists(string email)
        {
            return await context.Employees.AsNoTracking().AnyAsync(x => x.Email == email);
        }
    }
}
