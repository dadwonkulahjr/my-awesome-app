using ExampleEmpty.UI.ServiceCollectionExtention;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace ExampleEmpty.UI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            ExtendServiceCollection.ExtendService(services, Configuration);
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                DeveloperExceptionPageOptions developerExceptionPageOptions = new()
                {
                    SourceCodeLineCount = 1
                };

                app.UseDeveloperExceptionPage(developerExceptionPageOptions);
                ExtendApplicationBuilder.ExtendThisApplicationBuilder(app);
                        
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                ExtendApplicationBuilder.ExtendThisApplicationBuilder(app);
            }

           

        }
    }
}
