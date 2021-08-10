using ExampleEmpty.UI.Models;
using ExampleEmpty.UI.Models.Data;
using ExampleEmpty.UI.Models.Repository;
using ExampleEmpty.UI.Models.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleEmpty.UI.ServiceCollectionExtention
{
    public static class ExtendServices
    {
        public static IServiceCollection ExtendService(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddMvc(options =>
            {
                //Apply Authorization Globally!
                //to all controllers in the application!

                var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
                .AddControllersAsServices()
                .AddXmlSerializerFormatters();
            services.AddScoped<ICustomerRepository, CustomerSQLRepository>();
            services.AddDbContextPool<ApplicationDbContext>(options =>
                 options.UseSqlServer(
                     configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                //Code...
                options.Password.RequiredUniqueChars = 3;
                options.Password.RequiredLength = 10;

            }).AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash = true;
                options.LowercaseQueryStrings = true;
                options.LowercaseUrls = true;
            });

            services.AddOpenApiDocument(options =>
            {
                options.Title = "Example Empty Api Test!";
            });



            return services;

        }
    }
}
