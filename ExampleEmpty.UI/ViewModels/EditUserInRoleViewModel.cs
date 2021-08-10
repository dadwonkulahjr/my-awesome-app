namespace ExampleEmpty.UI.ViewModels
{
    public class EditUserInRoleViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public bool IsSelected { get; set; }
        public EditUserInRoleViewModel()
        {
            UserId = string.Empty;
            Username = string.Empty;
            IsSelected = false;
        }
        public EditUserInRoleViewModel(string userId)
        {
            UserId = userId;
            Username = string.Empty;
            IsSelected = false;
        }
        public EditUserInRoleViewModel(string userId, string username)
        {
            UserId = userId;
            Username = username;
            IsSelected = false;
        }
        public EditUserInRoleViewModel(string userId, string username, bool isSelected)
        {
            UserId = userId;
            Username = username;
            IsSelected = isSelected;
        }

    }
}
