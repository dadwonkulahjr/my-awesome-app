using ExampleEmpty.UI.Models;
using ExampleEmpty.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleEmpty.UI.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Authorize(Roles = "SuperAdministrator")]
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
        [Authorize(Roles = "Administrator")]
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
                ViewBag.DynamicRoleName = findRole.Name;

                var model = new List<EditUserInRoleViewModel>();
                foreach (var user in _userManager.Users.ToList())
                {
                    EditUserInRoleViewModel editUserInRoleViewModel = new(user.Id, user.UserName);
                    var checkUser = await _userManager.IsInRoleAsync(user, findRole.Name);
                    if (checkUser)
                    {
                        editUserInRoleViewModel.IsSelected = true;
                    }
                    else
                    {
                        editUserInRoleViewModel.IsSelected = false;
                    }

                    model.Add(editUserInRoleViewModel);
                }

                return View(model);

            }

        }


        [HttpPost]
        public async Task<IActionResult> EditUserInRole(List<EditUserInRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"The role with the specified Id {roleId} cannot be found.";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else { continue; }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("upsert", new { id = role.Id });
                    }
                }

            }
            return RedirectToAction("upsert", new { id = role.Id });
        }
    }
}
