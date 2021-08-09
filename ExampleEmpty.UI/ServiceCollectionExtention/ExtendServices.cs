using ExampleEmpty.UI.Models.Data;
using ExampleEmpty.UI.Models.Repository;
using ExampleEmpty.UI.Models.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
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
            services.AddMvc()
                .AddControllersAsServices()
                .AddXmlSerializerFormatters();
            services.AddScoped<ICustomerRepository, CustomerSQLRepository>();
            services.AddDbContextPool<ApplicationDbContext>(options =>
                 options.UseSqlServer(
                     configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                //Code...

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
