using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExampleEmpty.UI.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        [NotMapped]
        public string EncryptedCustomerId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public Gender? Gender { get; set; }
        public Customer(int customerId, string name, string address, Gender? gender)
        {
            CustomerId = customerId;
            Name = name;
            Address = address;
            Gender = gender;
        }

        public Customer(int customerId, string name, string address, string photoPath)
        {
            CustomerId = customerId;
            Name = name;
            Address = address;
            PhotoPath = photoPath;
        }

        public Customer(int customerId, string name, string address, Gender? gender, string photoPath)
        {
            CustomerId = customerId;
            Name = name;
            Address = address;
            Gender = gender;
            PhotoPath = photoPath;
        }
        public Customer()
        {
                
        }
        public string PhotoPath { get; set; }
    }
}
