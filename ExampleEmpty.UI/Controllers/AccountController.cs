using ExampleEmpty.UI.Models;
using ExampleEmpty.UI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExampleEmpty.UI.Controllers
{
    //Part 115
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdministrator")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdministrator")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Register(RegisterUserViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }

            //Register incoming user
            ApplicationUser newUser = new()
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                Gender = model.Gender
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, model.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { userId = newUser.Id, token }, Request.Scheme);

                _logger.Log(LogLevel.Information, confirmationLink);


                if (_signInManager.IsSignedIn(User) && User.IsInRole("Administrator") || User.IsInRole("SuperAdministrator"))
                {
                    return RedirectToAction("getallusers", "superadmin");
                }
                ViewBag.ErrorTitle = "Registration Successful";
                ViewBag.ErrorMessage = "Before you login, please confirm your email," +
                    "by clicking on the confirmation link we have emailed you";
                return View("error");
            }

            //Loop for errors and display all errors back to the user!
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }


            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "admin");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginUserViewModel model = new()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginUserViewModel model, string returnUrl)
        {
            model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid) { return View(model); }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && !user.EmailConfirmed && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError(string.Empty, "Email not confirmed yet.");
                return View("Login", model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("index", "admin");
                }
            }
            if (result.IsLockedOut)
            {
                return View(nameof(AccountLocked));
            }


            ModelState.AddModelError(string.Empty, "Invalid Login attempt");

            return View(model);

        }

        public IActionResult AccountLocked()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            var loginViewModel = new LoginUserViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider {remoteError}");
                return View("Login", loginViewModel);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error Loading external login information");
                return View("Login", loginViewModel);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            if (email != null)
            {
                user = await _userManager.FindByEmailAsync(email);

                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "email not confirmed yet.");
                    return View("Login", loginViewModel);
                }
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);



            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }
            else
            {

                if (email != null)
                {

                    if (user == null)
                    {
                        user = new ApplicationUser()
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            FirstName = info.Principal.FindFirstValue(ClaimTypes.Name),
                            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                            Gender = Gender.Male

                        };

                        await _userManager.CreateAsync(user);

                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, token }, Request.Scheme);

                        _logger.Log(LogLevel.Warning, confirmationLink);

                        ViewBag.ErrorTitle = "Registration Successful";
                        ViewBag.ErrorMessage = "Before you login, please confirm your email," +
                            "by clicking on the confirmation link we have emailed you";
                        return View("error");
                    }


                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return Redirect(returnUrl);
                }

                ViewBag.ErrorTittle = $"Email claim not received from {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on iamtuse.com";
                return View("Error");
            }
        }

        //[AllowAnonymous]
        //[AcceptVerbs("GET", "POST")]
        //public async Task<IActionResult> CheckUserEmailAddress(string email)
        //{
        //    var findEmail = await _userManager.FindByEmailAsync(email);

        //    if (findEmail == null)
        //    {
        //        return Json(true);
        //    }
        //    else
        //    {
        //        return Json($"The specified email address {email} is already taken in system.");
        //    }
        //}

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (token == null || userId == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The userId is incorrect {userId}";
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = $"The email {user.Email} cannot be confirmed.";
            return View("error");

        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }

            var user = await _userManager.FindByEmailAsync(model.EmailAddress);

            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetPasswordToken = Url.Action(nameof(ResetPassword), "Account", new { email = user.Email, token }, Request.Scheme);


                _logger.Log(LogLevel.Information, resetPasswordToken);
                return View(nameof(ForgetPasswordConfirmation));

            }

            return View(model);

        }

        [HttpGet, AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid token");
            }

            return View();
        }


        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

                if (result.Succeeded)
                {
                    if(await _userManager.IsLockedOutAsync(user))
                    {
                        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                    }

                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

            }

            return View(model);
        }

        [HttpGet, AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        [HttpGet, AllowAnonymous]
        public IActionResult ForgetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                if (!await _userManager.HasPasswordAsync(user))
                {
                    return RedirectToAction(nameof(AddPassword));
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }


            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                ViewBag.ErrorTitle = $"The user with the specified Id = {user.Id} cannot be found";
                ViewBag.ErrorMessage = $"Our support team is solving your problem.";
                return View("notfound");
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);


            if (!result.Succeeded)
            {
                foreach (var errror in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, errror.Description);
                }

                return View();
            }
            return View("changepasswordconfirmation");
        }

        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                if (await _userManager.HasPasswordAsync(user))
                {
                    return RedirectToAction(nameof(ChangePassword));
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }

            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    return View();
                }
            }
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction(nameof(AddPasswordConfirmation));

        }

        public IActionResult AddPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LogoutLink()
        {
            return View();
        }
    }
}
