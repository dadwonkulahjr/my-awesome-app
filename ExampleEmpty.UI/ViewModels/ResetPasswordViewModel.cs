using System.ComponentModel.DataAnnotations;

namespace ExampleEmpty.UI.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required, DataType(DataType.Password), Display(Name ="New Password")]
        public string NewPassword { get; set; }
        [Required, DataType(DataType.Password), Compare(nameof(NewPassword), ErrorMessage ="Password do not match."), Display(Name ="Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
        public string Email { get; set; }
    }
}
