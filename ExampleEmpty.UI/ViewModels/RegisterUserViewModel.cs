using ExampleEmpty.UI.AppUtilities;
using ExampleEmpty.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ExampleEmpty.UI.ViewModels
{
    public class RegisterUserViewModel
    {
        [EmailAddress, Required, Remote(action: "CheckUserEmailAddress", controller:"Account")]
        [EmailDomainValidator(emailDomain:"iamtuse.com", ErrorMessage ="Email domain must be iamtuse.com!")]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [Required, Display(Name ="Confirm Password"), Compare("Password", ErrorMessage ="Password and confirm password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        public Gender? Gender { get; set; }
        [Required(ErrorMessage ="First name is field is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is field is required.")]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        public string Fullname
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
