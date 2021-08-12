using ExampleEmpty.UI.Models;
using ExampleEmpty.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExampleEmpty.UI.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Authorize(Roles = "SuperAdministrator")]
    public class SuperAdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SuperAdminController> _logger;

        public SuperAdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<SuperAdminController> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
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
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var usersSorted = await _userManager.Users
                          .OrderBy(u => u.FirstName)
                          .ThenBy(u => u.LastName)
                          .ThenBy(u => u.UserName)
                          .ThenBy(u => u.Gender)
                          .ToListAsync();
            ViewData["AllUsers"] = "All Users";
            return View("allusers", usersSorted);
        }
        [HttpGet]
        public async Task<IActionResult> EditAppUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user with the specified Id = {id} cannot be found.";
                return View("NotFound");
            }
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);


            var model = new EditAppUserViewModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Gender = user.Gender,
                Address = user.Address,
                Username = user.UserName,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles
            };
            ViewData["UpdateUser"] = "Update User";
            return View("EditAppUser", model);
        }
        [HttpPost]
        public async Task<IActionResult> EditAppUser(EditAppUserViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user with the specified Id = {model.Id} cannot be found.";
                View("NotFound");
            }


            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.Address = model.Address;
            user.Gender = model.Gender;
            user.UserName = model.Username;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("getallusers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }



            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> RemoveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user with the specified Id = {userId} cannot be found.";
                return View("notfound");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("getallusers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }



            return View();
        }


        [HttpPost]
        public async Task<IActionResult> RemoveRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"The role with the specified Id = {roleId} cannot be found.";
                return View("notfound");
            }

            try
            {

                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("allroles");
                }


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }


                return View();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"An error occured {ex}");
                _logger.LogError($"Exception message {ex.Message}");

                ViewBag.ExTitle = $"{role.Name} cannot be deleted, because there are users in the role.";
                ViewBag.ExMessage = $"If you want to delete the {role.Name} role, make sure you removed all the users from in the role, before deleting the role.";
                return View("error");
            }

        }


        [HttpGet]
        public async Task<IActionResult> EditManageUserRole(string userId)
        {
            ViewBag.userId = userId;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user with the specified Id = {userId} cannot be found.";
                View("notfound");
            }

            var model = new List<ManageUserRoleViewModel>();
            var role = _roleManager.Roles;
            foreach (var r in role.ToList())
            {
                var manageUserRoleViewModel = new ManageUserRoleViewModel(r.Id, r.Name);

                if (await _userManager.IsInRoleAsync(user, r.Name))
                {
                    manageUserRoleViewModel.IsSelected = true;
                }
                else
                {
                    manageUserRoleViewModel.IsSelected = false;
                }

                model.Add(manageUserRoleViewModel);
            }


            return View("manageuserrole", model);
        }


        [HttpPost]
        public async Task<IActionResult> EditManageUserRole(List<ManageUserRoleViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The role with the specified Id {userId} cannot be found.";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var role = await _roleManager.FindByIdAsync(model[i].RoleId);
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
                        return RedirectToAction("editappuser", new { id = user.Id });
                    }
                }

            }
            return RedirectToAction("editappuser", new { id = user.Id });
        }

        [HttpGet]
        public async Task<IActionResult> EditManageUserClaim(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (userId == null)
            {
                ViewBag.ErrorMessage = $"The user with the specified Id = {userId} cannot be found.";
                return View("notfound");
            }

            var retrievedAllUserExistingClaims = await _userManager.GetClaimsAsync(user);
            var model = new ManageUserClaimViewModel()
            {
                UserId = userId
            };

            foreach (var claims in ClaimsFactory.GetClaims)
            {
                var userClaim = new UserClaims()
                {
                    ClaimType = claims.Type
                };

                if (retrievedAllUserExistingClaims.Any(c => c.Type == claims.Type))
                {
                    userClaim.IsSelected = true;
                }
                model.UserClaims.Add(userClaim);
            }



            return View("manageuserclaim", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditManageUserClaim(ManageUserClaimViewModel model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user with the specified Id = {userId} cannot be found.";
                return View("notfound");
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            var result = await _userManager.RemoveClaimsAsync(user, userClaims);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot remove user existing cliams");
                return View(model);
            }

            result = await _userManager
                                .AddClaimsAsync(user,
                                model.UserClaims.Where(c => c.IsSelected)
                                .Select(c => new Claim(c.ClaimType, c.ClaimType)));

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot add selected claims to user.");
                return View(model);
            }

            return RedirectToAction("editappuser", new { id = model.UserId });
        }
    }
}
