using ExampleEmpty.UI.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExampleEmpty.UI.ViewModels
{
    public class CustomerCreateViewModel
    {
        [NotMapped]
        public string EncryptedCustomerId { get; set; }
        public int CustomerId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public Gender? Gender { get; set; }
        public IFormFile Photo { get; set; }
        public CustomerCreateViewModel(int customerId, string name, string address, Gender? gender, IFormFile photo = null)
        {
            Name = name;
            Address = address;
            Gender = gender;
            Photo = photo;
            CustomerId = customerId;
        }
        public CustomerCreateViewModel(string name, string address, Gender? gender, IFormFile photo = null)
        {
            Name = name;
            Address = address;
            Gender = gender;
            Photo = photo;
            CustomerId = 0;
        }
        public CustomerCreateViewModel(int customerId)
        {
            CustomerId = customerId;
        }
        public CustomerCreateViewModel(IFormFile photo)
        {
            Photo = photo;
            CustomerId = 0;
        }
        public CustomerCreateViewModel(string name, string address, IFormFile photo = null)
        {
            Name = name;
            Address = address;
            Photo = photo;
            CustomerId = 0;
        }
        public CustomerCreateViewModel()
        {

            Name = "";
            Address = "";
            Gender = null;
            Photo = null;
            CustomerId = 0;
        }

    }
}
