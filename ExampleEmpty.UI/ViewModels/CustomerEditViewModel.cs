namespace ExampleEmpty.UI.ViewModels
{
    public class CustomerEditViewModel : CustomerCreateViewModel
    {
        public int Id { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}
