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
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger<UserController> logger;

        public UserController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, ILogger<UserController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._db = appDbContext;
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var users = userManager.Users.ToList().OrderBy(x=>x.Name);
            return View(users);
        }
        [HttpGet]
        //[Authorize(Policy = "CreateRolePolicy")]
        public async Task<IActionResult> CreateStaff()
        {
            //gender list
            var gender = new List<SelectListItem>();
            gender.Insert(0, new SelectListItem() { Text = "----Select Gender----", Value = "0" });
            gender.Insert(1, new SelectListItem() { Text = "Male", Value = "Male" });
            gender.Insert(2, new SelectListItem() { Text = "Female", Value = "Female" });
            gender.Insert(3, new SelectListItem() { Text = "Other", Value = "Other" });
            ViewBag.Gender = gender;

            //department list
            var DeptList = new List<SelectListItem>();
            DeptList = await _db.Departments.Select(x => new SelectListItem() { Text = x.Name, Value = x.Name }).Distinct().OrderBy(x => x.Value).ToListAsync();
            DeptList.Insert(0, new SelectListItem() { Text = "----Select Department----", Value = string.Empty });
            ViewBag.Department = DeptList;
            //state list
            var StateList = new List<SelectListItem>();
            StateList = await _db.StateLgas.Select(x => new SelectListItem() { Text = x.State, Value = x.State }).Distinct().OrderBy(x => x.Value).ToListAsync();
            StateList.Insert(0, new SelectListItem() { Text = "----Select State----", Value = string.Empty });
            ViewBag.State = StateList;
            //lga list
            var LgaList = new List<SelectListItem>();
            //LgaList = await _db.StateLgas.Select(x =>  x.Lga ).Distinct().ToListAsync();
            LgaList.Insert(0, new SelectListItem() { Text = "----Select LGA----", Value = string.Empty });
            ViewBag.Lga = LgaList;
            return View();
        }
        [HttpPost]
        //[Authorize(Policy = "CreateRolePolicy")]
        public async Task<IActionResult> CreateStaff(CreateStaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFilename = null, filePath = null;
                if (model.PhotoPath != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(model.PhotoPath.FileName);
                    string extension = Path.GetExtension(model.PhotoPath.FileName);
                    uniqueFilename = filename + "_" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + extension;
                    filePath = Path.Combine(webHostEnvironment.WebRootPath + "/images/", uniqueFilename);
                    //await model.PhotoPath.CopyToAsync(new FileStream(filePath, FileMode.Create)); //save image after record saved
                }
                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Department = model.Department,
                    Address = model.Address,
                    State = model.State,
                    Lga = model.Lga,
                    PhotoPath = uniqueFilename,
                    DOB = model.DOB.Date,
                    Date = DateTime.Now.Date,
                };
                var result = await userManager.CreateAsync(user, model.PhoneNumber);
                if (result.Succeeded)
                {
                    if (model.PhotoPath != null)
                    {
                        //save passport
                        await model.PhotoPath.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    }
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("confirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);
                   // logger.Log(LogLevel.Warning, confirmationLink);
                    ViewBag.ErrorTitle = "Registration successful";
                    ViewBag.ErrorMessage = "Confirmation link has been sent to your email, please confirm your email";
                    return View("error");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
            return View();
        }
        [HttpGet]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditStaff(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {id} cannot be found";
                return View("notFound");
            }
            //gender list
            var gender = new List<SelectListItem>();
            gender.Insert(0, new SelectListItem() { Text = "----Select Gender----", Value = "0" });
            gender.Insert(1, new SelectListItem() { Text = "Male", Value = "Male" });
            gender.Insert(2, new SelectListItem() { Text = "Female", Value = "Female" });
            gender.Insert(3, new SelectListItem() { Text = "Other", Value = "Other" });
            ViewBag.Gender = gender;

            //department list
            var DeptList = new List<SelectListItem>();
            DeptList = await _db.Departments.Select(x => new SelectListItem() { Text = x.Name, Value = x.Name }).Distinct().OrderBy(x=>x.Value).ToListAsync();
            DeptList.Insert(0, new SelectListItem() { Text = "----Select Department----", Value = string.Empty });
            ViewBag.Department = DeptList;
            //state list
            var StateList = new List<SelectListItem>();
            StateList = await _db.StateLgas.Select(x => new SelectListItem() { Text = x.State, Value = x.State }).Distinct().OrderBy(x => x.Value).ToListAsync();
            StateList.Insert(0, new SelectListItem() { Text = "----Select State----", Value = string.Empty });
            ViewBag.State = StateList;
            //lga list
            var LgaList = new List<SelectListItem>();
            //LgaList = await _db.StateLgas.Select(x =>  x.Lga ).Distinct().ToListAsync();
            LgaList.Insert(0, new SelectListItem() { Text = "----Select LGA----", Value = string.Empty });
            ViewBag.Lga = LgaList;

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);
            var model = new EditStaffViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Department = user.Department,
                State = user.State,
                Lga = user.Lga,
                Address = user.Address,
                OldPhotoPath = user.PhotoPath,
                DOB = user.DOB.Date,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles,
            };
            return View(model);
        }
        [HttpPost]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditStaff(EditStaffViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id: {model.Id} cannot be found";
                return View("notFound");
            }
            else
            {
                string uniqueFilename = null, filePath = null;
                if (model.PhotoPath != null)
                {
                    string filename = Path.GetFileNameWithoutExtension(model.PhotoPath.FileName);
                    string extension = Path.GetExtension(model.PhotoPath.FileName);
                    uniqueFilename = filename + "_" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + extension;
                    filePath = Path.Combine(webHostEnvironment.WebRootPath + "/images/", uniqueFilename);
                    //await model.PhotoPath.CopyToAsync(new FileStream(filePath, FileMode.Create)); //save image after record saved
                }
                //user.Id = model.Id;
                user.Email = model.Email;
                user.Name = model.Name;
                user.PhoneNumber = model.PhoneNumber;
                user.Gender = model.Gender;
                
                user.Department = model.Department;
                user.State = model.State;
                user.Lga = model.Lga == null ? model.InitialLga : model.Lga;
                user.Address = model.Address;
                
                user.DOB = model.DOB.Date;
                user.PhotoPath = model.PhotoPath != null ? uniqueFilename : model.OldPhotoPath;
                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (model.PhotoPath != null)
                    {
                        await model.PhotoPath.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    }
                    TempData["message"] = $"User with email: \"{model.Email}\" created successfully";
                    return RedirectToAction("index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"LoggedIn user not found";
                return View("notFound");
            }
            var model = new EditStaffViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Department = user.Department,
                State = user.State,
                Lga = user.Lga,
                Address = user.Address,
                OldPhotoPath = user.PhotoPath,
                DOB = user.DOB.Date
            };
            return View(model);
        }
        [HttpPost]
        //[Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(id);
                if (user == null)
                {
                    ViewBag.ErrorMessage = $"User with id {user.Id} cannot be found";
                    return View("notfound");
                }
                else
                {
                    var claims = await userManager.GetClaimsAsync(user);
                    var roles = await userManager.GetRolesAsync(user);
                    var result = await userManager.RemoveClaimsAsync(user, claims);

                    if (result.Succeeded)
                    {
                        var removeRoles = await userManager.RemoveFromRolesAsync(user, roles);
                        if (removeRoles.Succeeded)
                        {
                            var delete = await userManager.DeleteAsync(user);
                            if (delete.Succeeded)
                            {
                                TempData["message"] = $"User with id \"{id}\" deleted successfully";
                                return RedirectToAction("index");
                            }
                        }
                        foreach (var error in removeRoles.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View("index");
        }
    }
}
