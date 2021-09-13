using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExampleEmpty.UI.ViewModels
{
    public static class ClaimsFactory
    {
        public readonly static List<Claim> GetClaims = new()
        {
            new Claim("Create Role", "Create Role"),
            new Claim("Edit Role", "Edit Role"),
            new Claim("Delete Role", "Delete Role"),
            new Claim("View Role", "View Role")
        };
    }
}
