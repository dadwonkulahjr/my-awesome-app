using System.ComponentModel.DataAnnotations;

namespace ExampleEmpty.UI.ViewModels
{
    public class AddPasswordViewModel
    {
        [Display(Name = "New Password"), Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Display(Name ="Confirm Password"), Required, Compare(nameof(NewPassword), ErrorMessage ="Password do not match."), DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
