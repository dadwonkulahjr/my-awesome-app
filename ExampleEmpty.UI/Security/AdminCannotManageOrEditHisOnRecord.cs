using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExampleEmpty.UI.Security
{
    public class AdminCannotManageOrEditHisOnRecord : IAuthorizationRequirement { }
    public class SuperAdminHandler : IAuthorizationRequirement { }

    public class ManageSuperAdminAuthorizationRequirementHalder : AuthorizationHandler<SuperAdminHandler>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SuperAdminHandler requirement)
        {
            if (context.User.IsInRole("SuperAdministrator"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
    public class ManageAdminAuthorizationRequirementHandler : AuthorizationHandler<AdminCannotManageOrEditHisOnRecord>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminCannotManageOrEditHisOnRecord requirement)
        {
            var authorizationFiterContext = context.Resource as AuthorizationFilterContext;
            if (authorizationFiterContext == null) { return Task.CompletedTask; }

            string loginAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            string adminIdBeenEdited = authorizationFiterContext.HttpContext.Request.Query["userid"];
            if(context.User.IsInRole("Administrator") && loginAdminId != adminIdBeenEdited)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;

        }
    }

}
