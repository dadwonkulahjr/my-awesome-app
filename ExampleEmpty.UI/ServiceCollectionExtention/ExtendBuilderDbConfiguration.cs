using ExampleEmpty.UI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ExampleEmpty.UI.ServiceCollectionExtention
{
    public static class ExtendBuilderDbConfiguration
    {
        private static IEnumerable<Customer> GetListOfMyCustomerObj
        {
            get
            {
                return new List<Customer>()
                {
                    new(1, "Dad S Wonkulah Jr", "Caldwell Bongo Market", Gender.Male),
                    new(2, "Precious K Wonkulah", "Caldwell Bongo Market", Gender.Female),
                    new(3, "Darius F Wonkulah", "Caldwell Bongo Market", Gender.Male),
                    new(4, "Dacious F Wonkulah ", "Caldwell Bongo Market", Gender.Female),
                    new(5, "Leo Max", "Roberts Field Highway", Gender.Male),
                    new(6, "Test User 1", "Test Location", Gender.Unknown),
                    new(7, "Test User 2", "Test Location ", Gender.Unknown)
                };
            }
        }

        public static ModelBuilder SeedDbWithInitialData(this ModelBuilder incomingBuilderObj)
        {
            incomingBuilderObj.Entity<Customer>()
                              .HasData(GetListOfMyCustomerObj);
            return incomingBuilderObj;

        }
    }
}
