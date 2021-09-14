using System.ComponentModel.DataAnnotations;

namespace ExampleEmpty.UI.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Display(Name ="New Password"), Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Display(Name = "Current Password"), Required, DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [Display(Name = "Confirm Password"), Required, DataType(DataType.Password), Compare(nameof(CurrentPassword), ErrorMessage ="Password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
