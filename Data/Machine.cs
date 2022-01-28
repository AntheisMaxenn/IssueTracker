using System;
using System.Collections.Generic;

namespace IssueTracker.Data
{
    public partial class Machine
    {
        public Machine()
        {
            Issues = new HashSet<Issue>();
        }

        public int MachineId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public virtual ICollection<Issue> Issues { get; set; }
    }
}
