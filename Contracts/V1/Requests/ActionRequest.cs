namespace IssueTracker.Contracts.V1.Requests
{
    public class ActionRequest
    {
        public string Involvement { get; set; } = null!;
        public int IssueId { get; set; }
        public string EmployeeId { get; set; }

        public int MachineId { get; set; }
        public int ComponentId { get; set; }
        public int LocationId { get; set; }

    }
}
