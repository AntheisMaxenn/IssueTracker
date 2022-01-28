using System;
using System.Collections.Generic;

namespace IssueTracker.Data
{
    public partial class Component
    {
        public Component()
        {
            Issues = new HashSet<Issue>();
        }

        public int ComponentId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public virtual ICollection<Issue> Issues { get; set; }
    }
}
