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
                    new(1, "Tom Smith", "Jallah Town", Gender.Male),
                    new(2, "Sara Collins", "Sinkor", Gender.Female),
                    new(3, "Test User 1", "Bye Pass", Gender.Unknown)
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
