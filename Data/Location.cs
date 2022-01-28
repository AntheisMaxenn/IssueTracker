using System;
using System.Collections.Generic;

namespace IssueTracker.Data
{
    public partial class Location
    {
        public Location()
        {
            Issues = new HashSet<Issue>();
        }

        public int LocationId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public virtual ICollection<Issue> Issues { get; set; }
    }
}
