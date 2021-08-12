using ExampleEmpty.UI.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExampleEmpty.UI.AppUtilities;

namespace ExampleEmpty.UI.ViewModels
{
    public class EditAppUserViewModel
    {
        public EditAppUserViewModel(string id, string firstName, string lastName, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
        public EditAppUserViewModel(string id, string firstName, string lastName, string email, string address)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Address = address;
        }
        public EditAppUserViewModel(string id, string firstName, string lastName, Gender? gender)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
        }
        public EditAppUserViewModel(string id, string firstName, string lastName, Gender? gender, string address, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Gender = gender;
            Address = address;
            Email = email;
        }
        public EditAppUserViewModel()
        {
            Roles = new List<string>();
            Claims = new();
        }
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public Gender? Gender { get; set; }
        [Required]
        public string Address { get; set; }
        [Required, EmailAddress]
        [EmailDomainValidator(emailDomain: "iamtuse.com", ErrorMessage = "Email domain must be iamtuse.com!")]
        public string Email { get; set; }
        [Required]
        public IList<string> Roles { get; set; }
        public List<string> Claims { get; set; }
        [EmailDomainValidator(emailDomain: "iamtuse.com", ErrorMessage = "Email domain must be iamtuse.com!")]
        [Required]
        public string Username { get; set; }
    }
}
