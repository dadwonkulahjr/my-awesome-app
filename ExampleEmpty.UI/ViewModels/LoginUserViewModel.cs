using ExampleEmpty.UI.AppUtilities;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExampleEmpty.UI.ViewModels
{
    public class LoginUserViewModel
    {
        [Required]
        [EmailAddress]
        [EmailDomainValidator(emailDomain: "iamtuse.com", ErrorMessage = "Email domain must be iamtuse.com for successful login!")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
