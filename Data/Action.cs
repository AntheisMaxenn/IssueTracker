using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Data
{
    public partial class Action
    {
        public int ActionId { get; set; }
        public string Involvement { get; set; } = null!;
        public DateTime DateTime { get; set; }
        public int IssueId { get; set; }

        public string EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public virtual Employee Employee { get; set; } = null!;
        public virtual Issue Issue { get; set; } = null!;
    }
}
