using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleEmpty.UI.ViewModels
{
    public class ManageUserClaimViewModel
    {
        public ManageUserClaimViewModel()
        {
            UserClaims = new();
        }
        public string UserId { get; set; }
        public List<UserClaims> UserClaims { get; set; }
    }
}
