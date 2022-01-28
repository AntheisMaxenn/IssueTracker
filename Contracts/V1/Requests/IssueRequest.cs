using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Contracts.V1.Requests
{
    public class IssueRequest
    {
        public int? IssueId { get; set; }
        public string Description { get; set; } = null!;
        public string? Impact { get; set; }
        public string? Danger { get; set; }
        public string? Classification { get; set; }
        public string Status { get; set; } = null!;

        //public DateTime DateTime { get; set; }
        public int MachineId { get; set; }
        public int ComponentId { get; set; }
        public int LocationId { get; set; }
        public string EmployeeId { get; set; }

        //public virtual Component Component { get; set; } = null!;
        //public virtual Location Location { get; set; } = null!;
        //public virtual Machine Machine { get; set; } = null!;
        //public virtual ICollection<Action> Actions { get; set; }

        [MaxLength(20)]
        public string Involvement { get; set; }

    }
}
