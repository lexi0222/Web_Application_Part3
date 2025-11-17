using System.ComponentModel.DataAnnotations;

namespace Web_Application_Part3.Models
{
    public class Login
    {
        
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }

            [Required]
            public string Role { get; set; } // Admin or Lecturer
        }
    }

