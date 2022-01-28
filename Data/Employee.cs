using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Data
{
    public partial class Employee
    {
        public Employee()
        {
            Actions = new HashSet<Action>();
        }

        public string EmployeeId { get; set; }
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Position { get; set; } = null!;

        public virtual ICollection<Action> Actions { get; set; }
        
        [ForeignKey(nameof(EmployeeId))]
        public virtual IdentityUser User { get; set; }
    }
}
