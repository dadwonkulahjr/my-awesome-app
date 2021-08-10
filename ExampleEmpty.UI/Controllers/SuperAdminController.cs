using ExampleEmpty.UI.Models;
using ExampleEmpty.UI.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleEmpty.UI.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public SuperAdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
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

            if (roleFound == null)
            {
                ViewBag.ErrorMessage = $"The role with the specified Id {id} cannot be found.";
                return View("NotFound");
            }

            CreateRoleViewModel model = new(roleFound.Id, roleFound.Name);

            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, roleFound.Name))
                {

                    model.Users.Add(user.UserName);
                }
            }

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Upsert(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }


            if (model.RoleId == null)
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
                    var result = await _roleManager.UpdateAsync(findRole);


                    if (result.Succeeded)
                    {
                        return RedirectToAction("allroles", "superadmin");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                }
                else
                {
                    ViewBag.ErrorMessage = $"The role with the specified Id {findRole.Id} cannot be found.";
                    return View("NotFound");
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

        [HttpGet]
        public async Task<IActionResult> EditUserInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var findRole = await _roleManager.FindByIdAsync(roleId);

            if (findRole == null)
            {
                ViewBag.ErrorMessage = $"The role with the specified Id = {roleId} cannot be found.";
                return View("NotFound");
            }
            else
            {
                List<EditUserInRoleViewModel> editUserInRoleViewModel = new();
                if (editUserInRoleViewModel.Count == 0)
                {
                    EditUserInRoleViewModel editUserInRoleModel = new();
                    _userManager.Users.ToList().ForEach(async (a) =>
                    {
                        a.Id = editUserInRoleModel.UserId;
                        a.UserName = editUserInRoleModel.Username;
                        if (await _userManager.IsInRoleAsync(a, findRole.Name))
                        {
                            editUserInRoleModel.IsSelected = true;
                        }
                        else
                        {
                            editUserInRoleModel.IsSelected = true;
                        }

                        editUserInRoleViewModel.Add(editUserInRoleModel);
                    });
                    if (editUserInRoleViewModel.Count == 0)
                    {
                        return RedirectToAction("upsert", new { id = roleId });
                    }
                    else
                    {
                        return View(editUserInRoleViewModel);
                    }
                }
            }


            return RedirectToAction("upsert", new { id = roleId });
        }
    }
}
