using Microsoft.AspNetCore.Builder;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace ExampleEmpty.UI.ServiceCollectionExtention
{
    public static class ExtendApplicationBuilder
    {
        public static IApplicationBuilder ExtendThisApplicationBuilder(this IApplicationBuilder app)
        {
           
            app.UseStaticFiles();

            app.UseRouting();
            /*
             * Endpoint ExampleEmpty.UI.Controllers.AdminController.Upsert (ExampleEmpty.UI)
             contains authorization metadata, but a middleware was not found that supports authorization.
            Configure your application startup by adding app.UseAuthorization() inside the call to Configure(..) in the application startup code. The call to app.UseAuthorization() must appear between app.UseRouting() and app.UseEndpoints(. **/
             
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseOpenApi();
            app.UseSwaggerUi3();

          
            app.UseEndpoints((_config) =>
            {
                _config.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Admin}/{action=Index}/{id?}"
                    );
            });
          
            return app;
        }
    }
}
