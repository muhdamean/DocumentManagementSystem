using DocumentManagementSystem.Models;
using DocumentManagementSystem.Models.ViewModels;
using DocumentManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ILogger<AccountController> logger;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IMailService mailService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, ILogger<AccountController> logger, AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IMailService mailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.logger = logger;
            this._db = appDbContext;
            this.webHostEnvironment = webHostEnvironment;
            this.mailService = mailService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SignIn(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel model, string ReturnUrl)
        {
            //begin set role here
            var users = userManager.Users.ToList();
            if (users != null)
            {
                string role = "User";
                foreach (var item in users)
                {
                    var userRole = await roleManager.FindByIdAsync(item.Id);
                    if (userRole == null)
                    {
                        await userManager.AddToRoleAsync(item, role);
                    }

                }
            }//end set role here

            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View(model);
                }
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else if(string.IsNullOrEmpty(ReturnUrl))
                    {
                        return RedirectToAction("index", "admin");
                    }
                    else
                    {
                        return View(model);
                    }
                }
                //if (result.IsLockedOut)
                //{
                //    return View("AccountLocked");
                //}
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SignOuts()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("externalLoginCallBack", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("signin", loginViewModel);
            }
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information");
                return View("signin", loginViewModel);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;
            if (email != null)
            { 
                user = await userManager.FindByEmailAsync(email); 
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View("signin", loginViewModel);
                }
            }
            var siginInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (siginInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                if (email != null)
                {
                    user = await userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        await userManager.CreateAsync(user);
                        //external email confirmation here
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("confirmemail", "account", new { userId = user.Id, token = token }, Request.Scheme);
                        //logger.Log(LogLevel.Warning, confirmationLink);
                        //if (signInManager.IsSignedIn(User) && User.IsInRole("admin"))
                        //{
                        //    return RedirectToAction("index", "admin");
                        //}
                        ViewBag.ErrorTitle = "Registration successful";
                        ViewBag.ErrorMessage = "Before you can login, please confirm your email by clicking on the confirmation link we have emailed you";
                        return View("error");
                    }
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
                ViewBag.ErrorTitle = $"Email claim not recieved from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on mailcontact2016@gmail.com";
                return View("error");
            }
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var check = await userManager.FindByEmailAsync(email);
            if (check == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            var gender = new List<SelectListItem>();
            gender.Insert(0, new SelectListItem() { Text = "-Gender-", Value = "0" });
            gender.Insert(1, new SelectListItem() { Text = "Male", Value = "Male" });
            gender.Insert(2, new SelectListItem() { Text = "Female", Value = "Female" });
            gender.Insert(3, new SelectListItem() { Text = "Other", Value = "Other" });
            ViewBag.Gender = gender;
            
            //department list
            var DeptList = new List<SelectListItem>();
            DeptList = await _db.Departments.Select(x => new SelectListItem() { Text = x.Name, Value = x.Name }).Distinct().OrderBy(x => x.Value).ToListAsync();
            DeptList.Insert(0, new SelectListItem() { Text = "-Department-", Value = string.Empty });
            ViewBag.Department = DeptList;
            //state list
            var StateList = new List<SelectListItem>();
            StateList = await _db.StateLgas.Select(x => new SelectListItem() { Text = x.State, Value = x.State }).Distinct().OrderBy(x=>x.Value).ToListAsync();
            StateList.Insert(0, new SelectListItem() { Text = "-Select State-", Value = string.Empty });
            ViewBag.State = StateList;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(CreateStaffViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    Name = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Gender=model.Gender,
                    State=model.State,
                    Lga=model.Lga,
                    Address=model.Address,
                    IsActive=true,
                    Department=model.Department,
                    Date=DateTime.Now.Date
                };
                var result = await userManager.CreateAsync(user, model.PhoneNumber);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("confirmEmail", "account", new { userId = user.Id, token = token }, Request.Scheme);
                    //logger.Log(LogLevel.Warning, confirmationLink);
                    //send mail
                    //mailRequest.Subject = "Email Confirmation";
                    //mailRequest.ToEmail = ApplicationUsers.Email;
                    //mailRequest.Body = $@"<p>Hi {ApplicationUsers.FullName}, your registration with Staff ID {ApplicationUsers.StaffID} is recieved.</p>
                    //                        <p>Kindly click on the link below to confirm your email <br/></p>
                    //                     <a href={confirmationLink}>Confirm your email</a>.<br/><br/>
                    //                    Please kindly disregard this email if you did not initiate the above.  Thanks!<br/>";
                    //await mailService.SendEmailAsync(mailRequest);

                    ViewBag.ErrorTitle = "Registration successful";
                    ViewBag.ErrorMessage = "Confirmation link has been sent to your email, please confirm your email";
                    return View("Error");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }

            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null && token == null)
            {
                // ViewBag.ErrorTitle = "Invalid";
                ViewBag.ErrorMessage = "Email cannot be confirmed";
                return View("error");
            }
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorTitle = "Invalid";
                ViewBag.ErrorMessage = $"The User Id {userId} is invalid";
                return View("error");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }
            ViewBag.ErrorTitle = "Invalid";
            ViewBag.ErrorMessage = "Email cannot be confirmed";
            return View("error");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("resetpassword", "account", new { email = model.Email, token = token }, Request.Scheme);
                    logger.Log(LogLevel.Warning, passwordResetLink); //send mail here
                    return View("sendlinkconfirmation");
                }
                else if (user == null)
                {
                    ViewBag.ErrorTitle = "Invalid Email";
                    ViewBag.ErrorMessage = $"Email {model.Email} not found";
                    return View("notfound");
                }
                return View(model);
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            if (email == null || token == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid password reset token");
            }
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var model = new ResetPasswordViewModel
                {
                    Email = user.Email,
                    Token = token
                };
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                    if (result.Succeeded)
                    {
                        if (await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }

                        return View("resetpasswordconfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                else if (user == null)
                {
                    ViewBag.ErrorTitle = "Invalid Email";
                    ViewBag.ErrorMessage = $"Email {model.Email} not found";
                    return View("notfound");
                }
                return View(model);
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);
            var userHasPassword = await userManager.HasPasswordAsync(user);
            if (!userHasPassword)
            {
                return RedirectToAction("addpassword");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    RedirectToAction("signin");
                }
                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }
                await signInManager.RefreshSignInAsync(user);
                return View("resetpasswordconfirmation");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AddPassword()
        {
            var user = await userManager.GetUserAsync(User);
            var userHasPassword = await userManager.HasPasswordAsync(user);
            if (userHasPassword)
            {
                return RedirectToAction("changepassword");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("signin");
                }
                var result = await userManager.AddPasswordAsync(user, model.NewPassword);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }
                await signInManager.RefreshSignInAsync(user);
                return View("resetpasswordconfirmation");
            }
            return View(model);
        }
    }
}
