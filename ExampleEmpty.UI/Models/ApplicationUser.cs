using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ExampleEmpty.UI.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public Gender? Gender { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Address { get; set; }
    }
}
