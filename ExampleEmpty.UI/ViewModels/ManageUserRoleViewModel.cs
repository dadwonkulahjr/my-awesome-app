namespace ExampleEmpty.UI.ViewModels
{
    public class ManageUserRoleViewModel
    {
        public string RoleId { get; set; }
        public bool IsSelected { get; set; }
        public string RoleName { get; set; }
        public ManageUserRoleViewModel()
        {
            RoleId = string.Empty;
            IsSelected = false;
            RoleName = string.Empty;
        }
        public ManageUserRoleViewModel(string roleId, string roleName)
        {
            RoleId = roleId;
            RoleName = roleName;
        }
        public ManageUserRoleViewModel(string roleId, bool isSelected)
        {
            RoleId = roleId;
            IsSelected = isSelected;
        }
        public ManageUserRoleViewModel(string roleId, bool isSelected, string roleName)
        {
            RoleId = roleId;
            IsSelected = isSelected;
            RoleName = roleName;
        }

        public ManageUserRoleViewModel(string roleId, string roleName, bool isSelected)
        {
            RoleId = roleId;
            RoleName = roleName;
            IsSelected = isSelected;
        }
    }
}
