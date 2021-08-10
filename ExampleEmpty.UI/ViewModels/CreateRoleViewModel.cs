using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExampleEmpty.UI.ViewModels
{
    public class CreateRoleViewModel
    {
        [Display(Name = "Role Id")]
        public string RoleId { get; set; }
        [Required(ErrorMessage = "Role name is required."), Display(Name = "Role name")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }

        public CreateRoleViewModel(string roleId, string roleName, List<string> users)
        {
            RoleId = roleId;
            RoleName = roleName;
            Users = users;
            Users = new();
        }
        public CreateRoleViewModel(string roleId, string roleName)
        {
            RoleId = roleId;
            RoleName = roleName;
            Users = new();
        }
        public CreateRoleViewModel(string roleName)
        {
            RoleName = roleName;
            Users = new();
        }
        public CreateRoleViewModel(string roleName, List<string> users)
        {
            RoleName = roleName;
            Users = users;
        }
        public CreateRoleViewModel(List<string> users)
        {
            Users = users;
        }
        public CreateRoleViewModel()
        {
            RoleName = string.Empty;
            RoleId = string.Empty;
            Users = new();
        }
    }
}
