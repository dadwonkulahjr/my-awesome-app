using ExampleEmpty.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleEmpty.UI.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public SuperAdminController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(string id)
        {
            if (id == null)
            {
                id = string.Empty;
                CreateRoleViewModel role = new()
                {
                    RoleId = id
                };
                return View(role);
            }
            var roleFound = await _roleManager.FindByIdAsync(id);
            if (roleFound != null)
            {
                CreateRoleViewModel role = new()
                {
                    RoleId = roleFound.Id,
                    RoleName = roleFound.Name
                };
                return View(role);
            }

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Upsert(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }


            if (model.RoleId == string.Empty)
            {

                IdentityRole newRole = new()
                {
                    Name = model.RoleName
                };

                var result = await _roleManager.CreateAsync(newRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("allroles", "superadmin");
                }

                foreach (var errors in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, errors.Description);
                }

                return View(model);
            }
            else
            {
                var findRole = await _roleManager.FindByIdAsync(model.RoleId);

                if (findRole != null)
                {
                    findRole.Name = model.RoleName;
                    
                }
            }

            return View(model);

        }

        public IActionResult AllRoles()
        {
            var listOfRoles = _roleManager.Roles
                                          .OrderBy(o => o.Name)
                                          .ToList();

            ViewData["Roles"] = "All Roles";
            return View("AllRoles", listOfRoles);
        }
    }
}
