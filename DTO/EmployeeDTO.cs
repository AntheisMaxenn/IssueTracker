using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Domain
{
    public class EmployeeDTO
    {
        public EmployeeDTO(string email, string password, string firstName, string lastName, string position)
        {
            Email = email;
            Password = password;
            FirstName = firstName;
            LastName = lastName;
            Position = position;
            IsAdmin = false;
        }

        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Email { get; set; }

        [Required]
        [MaxLength(30)]
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

        public bool IsAdmin { get; set;  }



    }
}
