using System.ComponentModel.DataAnnotations;

namespace ExampleEmpty.UI.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required, EmailAddress, Display(Name ="Email Address")]
        public string EmailAddress { get; set; }
    }
}
