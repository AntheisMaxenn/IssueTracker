namespace IssueTracker.Contracts.V1
{
    public class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string Base = Root + "/" + Version;

        public static class Locations
        {
            // GetAll + search
            public const string GetAll = Base + "/locations";

            // Update
            public const string Update = Base + "/locations";

            // Delete
            public const string Delete = Base + "/locations/{locationId}";

            // Get
            public const string Get = Base + "/locations/{locationId}";

            // Create
            public const string Create = Base + "/locations";
        }

        public static class Machines
        {
            // GetAll + search
            public const string GetAll = Base + "/machines";

            // Update
            public const string Update = Base + "/machines";

            // Delete
            public const string Delete = Base + "/machines/{machineId}";

            // Get
            public const string Get = Base + "/machines/{machineId}";

            // Create
            public const string Create = Base + "/machines";
        }

        public static class Components
        {
            // GetAll + search
            public const string GetAll = Base + "/components";

            // Update
            public const string Update = Base + "/components";

            // Delete
            public const string Delete = Base + "/components/{componentId}";

            // Get
            public const string Get = Base + "/components/{componentId}";

            // Create
            public const string Create = Base + "/components";
        }

        public static class Employee
        {
            // GetAll + search
            public const string GetAll = Base + "/employees"; 

            // Update
            public const string Update = Base + "/employees/{employeeId}";

            // Delete
            public const string Delete = Base + "/employees/{employeeId}";

            // Get
            public const string GetById = Base + "/employees/{employeeId}";

            // Create
            public const string Create = Base + "/employees";

        }

        public static class Identity
        {
            public const string Register = Base + "/identity/register";
            
            public const string Login = Base + "/identity/login";
            
            public const string Refresh = Base + "/identity/refresh";
        }

        public static class Authorization
        {
            // Roles

            public const string RemoveUserFromRoleAsync = Base + "/authorization/role/removeuser";

            public const string AddUserToRoleAsync = Base + "/authorization/role/adduser";
            
            public const string AddClaimToRoleAsync = Base + "/authorization/role/addclaim";

            public const string RemoveClaimFromRoleAsync = Base + "/authorization/role/removeclaim";
            
            public const string CreateNewRole = Base + "/authorization/role/new";
            
            // Claims
            public const string RemoveClaimFromUserAsync = Base + "/authorization/claim/user"; // Delete
            public const string AddClaimToUserAsync = Base + "/authorization/claim/user"; // Post
            

        }

        public static class Issue
        {
            // Create Issue + Action.
            public const string CreateIssueAction = Base + "/issue";

            // GetAll + search.
            public const string GetAll = Base + "/issues";

            // Update.
            public const string Update = Base + "/issues";

            // Delete.
            public const string Delete = Base + "/issues/{issueId}";

            // Get.
            public const string GetById = Base + "/issues/{issueId}";
            
            // GetDetailed.
            public const string GetDetailedById = Base + "/issues/detailed/{issueId}";


        }

        public static class Action
        {
            
            // GetAllActionsOfIssue, paged.
            public const string GetRespectiveActions = Base + "/actions/{issueId}";

            // Create.
            public const string Create = Base + "/actions";

            // Update.
            //public const string Update = Base + "/actions";

            // Delete.
            public const string Delete = Base + "/actions/{actionId}";

        }

    }
}
