using System.ComponentModel.DataAnnotations;

namespace ExampleEmpty.UI.ViewModels
{
    public class CreateRoleViewModel
    {
        public string RoleId { get; set; }
        [Required(ErrorMessage ="Role name is required."), Display(Name ="Role name")]
        public string RoleName { get; set; }
        public CreateRoleViewModel(string roleName)
        {
            RoleName = roleName;
        }
        public CreateRoleViewModel()
        {
            RoleName = string.Empty;
        }
    }
}
