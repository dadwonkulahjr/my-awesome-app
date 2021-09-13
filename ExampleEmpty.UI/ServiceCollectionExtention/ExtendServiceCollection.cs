using ExampleEmpty.UI.DataProtectionTokenManager;
using ExampleEmpty.UI.Models;
using ExampleEmpty.UI.Models.Data;
using ExampleEmpty.UI.Models.Repository;
using ExampleEmpty.UI.Models.Repository.IRepository;
using ExampleEmpty.UI.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public static class ExtendServiceCollection
    {
        static IEnumerable<string> GetSuperAdminAndAdminPolicy
        {
            get { return new string[] { "SuperAdministrator", "Administrator", "Test" }; }
        }
        public static IServiceCollection ExtendService(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/superadmin/accessdenied");
            });

            services.AddMvc(options =>
            {
                //Apply Authorization Globally!
                //to all controllers in the application!

                var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddControllersAsServices()
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
                options.SignIn.RequireConfirmedEmail = true;


                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation";

            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<CustomEmailConfirmationTokenProvider<ApplicationUser>>("CustomEmailConfirmation");

            //Overide token life span for email confirmation token
            //to 3 days before it become expire.
            services.Configure<CustomEmailConfirmationTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(3);
            });

            //Change token life span for all tokens
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(5);
            });

           

            services.AddSingleton<IAuthorizationHandler, ManageAdminAuthorizationRequirementHandler>();
            services.AddSingleton<IAuthorizationHandler, ManageSuperAdminAuthorizationRequirementHalder>();

            services.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash = true;
                //options.LowercaseQueryStrings = true;
                //options.LowercaseUrls = true;
            });

            services.AddOpenApiDocument(options =>
            {
                options.Title = "Example Empty Api Test!";
            });


            services.AddAuthorization(aOptions =>
            {
                aOptions.AddPolicy("CreateRolePolicy", aPolicy =>
                {
                    aPolicy.RequireClaim("Create Role", "true");
                });

                aOptions.AddPolicy("EditRolePolicy", aPolicy =>
                {
                    aPolicy.AddRequirements(new AdminCannotManageOrEditHisOnRecord());
                });

                aOptions.AddPolicy("DeleteRolePolicy", aPolicy =>
                {
                    aPolicy.RequireClaim("Delete Role", "true");
                });

                aOptions.AddPolicy("ViewRolePolicy", aPolicy =>
                {
                    aPolicy.RequireClaim("View Role", "true");
                });

                aOptions.AddPolicy("SuperAdminPolicy", aPolicy =>
                {
                    aPolicy.RequireRole(GetSuperAdminAndAdminPolicy);
                });

                aOptions.AddPolicy("TestThisPolicy", options =>
                {
                    options.RequireAssertion((context) =>
                    {
                        return context.User.IsInRole("SuperAdministrator") && context.User.HasClaim(c => c.Type == "Edit Role" && c.Value == "true") || context.User.IsInRole("Test");
                    });
                });

            });

            services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        options.ClientId =
                        "396572300480-m2in5tbmmcfp224qr42s8brc23iqnl23.apps.googleusercontent.com";
                        options.ClientSecret = "XLC1q42cVsTBXEgm801n7yVm";
                    })
                    .AddFacebook(options =>
                    {
                        options.AppId = "219777956833959";
                        options.AppSecret = "9e36f814444c091ab9140fb92b46b552";
                    });

            services.AddSingleton<DataProtectionPurposeStrings>();
                   

            return services;

        }
    }
}
