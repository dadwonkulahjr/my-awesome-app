using Microsoft.AspNetCore.Builder;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace ExampleEmpty.UI.ServiceCollectionExtention
{
    public static class ExtendConfigureServices
    {
        public static IApplicationBuilder ExtendThisApplicationBuilder(this IApplicationBuilder app)
        {

            app.UseStaticFiles();
            app.UseRouting();

            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseAuthentication();

            app.UseEndpoints((_config) =>
            {
                _config.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Admin}/{action=Index}/{id?}"
                    );
            });
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello, World", CancellationToken.None);
            //});
            return app;
        }
    }
}
