using System;
using System.Collections.Generic;

namespace IssueTracker.Data
{
    public partial class Issue
    {
        public Issue()
        {
            Actions = new HashSet<Action>();
        }

        public int IssueId { get; set; }
        public string Description { get; set; } = null!;
        public string? Impact { get; set; }
        public string? Danger { get; set; }
        public string? Classification { get; set; }
        public string Status { get; set; } = null!;
        public DateTime DateTime { get; set; }
        public int? MachineId { get; set; }
        public int? ComponentId { get; set; }
        public int? LocationId { get; set; }

        public virtual Component Component { get; set; } = null!;
        public virtual Location Location { get; set; } = null!;
        public virtual Machine Machine { get; set; } = null!;
        public virtual ICollection<Action> Actions { get; set; }
    }
}
