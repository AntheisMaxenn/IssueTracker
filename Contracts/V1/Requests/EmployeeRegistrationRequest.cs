using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Contracts.V1.Requests
{
    public class EmployeeRegistrationRequest
    {
        public EmployeeRegistrationRequest(string email, string password, string firstName, string lastName, string position)
        {
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Position = position;
        }

        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }    
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }    

        [Required]
        [MaxLength(50)]
        public string Position { get; set; }


    }
}
