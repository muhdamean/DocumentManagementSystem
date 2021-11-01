using DocumentManagementSystem.Models;
using DocumentManagementSystem.Models.ViewModels;
using DocumentManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Controllers
{
    
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdminController> logger;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor contextAccessor;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<AdminController> logger, AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor contextAccessor)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
            this._db = appDbContext;
            this.webHostEnvironment = webHostEnvironment;
            this.contextAccessor = contextAccessor;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            //document type count
            var DocTypeCount = from b in _db.Documents
                               group b by b.Type into g
                               select new DocTypeCount
                               {
                                   DocType = g.Key,
                                   Count = g.Count()
                               };

            List<DocTypeCount> docTypeCounts = new List<DocTypeCount>();
            foreach (var d in DocTypeCount)
            {
                var docs = new DocTypeCount
                {
                    DocType = d.DocType,
                    Count = d.Count
                };
                docTypeCounts.Add(docs);
            }

            //department list
            var dept= _db.Departments.ToList();
            List<DepartmentViewModel> departmentList = new List<DepartmentViewModel>();
            foreach (var d in dept)
            {
                var department = new DepartmentViewModel
                {
                    Name = d.Name,
                    Status = d.IsActive,
                    SubmittedBy = d.SubmittedBy,
                    DateCreated = d.DateCreated
                };
                departmentList.Add(department);
            }
            //total comments
            var commentCount = _db.Comments.Count();
            //user profile
            var user =await userManager.FindByEmailAsync(User.Identity.Name);
            UserViewModel UserProfile = new UserViewModel
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.PhoneNumber,
                Address = user.Address,
                Department = user.Department,
                DOB = user.DOB,
                Gender = user.Gender,
                IsActive = user.IsActive,
                State = user.State,
                Lga = user.Lga,
                PhotoPath = user.PhotoPath
            };
            //List<UserViewModel> userViewModels = new List<UserViewModel>();
            //userViewModels.Add(UserProfile);
            //document activity
            var document = _db.Documents.Include(x => x.ApplicationUser).ToList().OrderByDescending(x => x.Date);
            List<DocumentViewModel> documentList = new List<DocumentViewModel>();
            foreach (var item in document)
            {
                DocumentViewModel documentViewModel = new DocumentViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Size = item.Size,
                    Type = item.Type,
                    IsNew = item.IsNew,
                    UploadId = item.UploadId,
                    Status = item.Status,
                    Date = item.Date.Date,
                    UploadedBy = item.UserId,
                    UserName = item.ApplicationUser.Name,
                    Phone = item.ApplicationUser.PhoneNumber,
                    Passport = item.ApplicationUser.PhotoPath,
                    Email = item.ApplicationUser.Email,
                    UploadComment = item.UploadComment
                };
                documentList.Add(documentViewModel);
            }
            IndexViewModel indexViewModel = new IndexViewModel();
            indexViewModel.GetDocumentViewModels = documentList;
            indexViewModel.GetDepartmentViewModels = departmentList;
            indexViewModel.GetUserViewModels = UserProfile;
            indexViewModel.TotalComments = commentCount;
            indexViewModel.DocTypeCounts = docTypeCounts;

            //doc type count
            indexViewModel.AllDocCount = _db.Documents.Count();
            indexViewModel.PdfCount = _db.Documents.Where(x => x.Type == "PDF").Count();
            indexViewModel.WordCount = _db.Documents.Where(x => x.Type == "DOCX").Count();
            indexViewModel.PptCount = _db.Documents.Where(x => x.Type == "PPT").Count();
            indexViewModel.ExlxCount = _db.Documents.Where(x => x.Type == "EXLX").Count();
            return View(indexViewModel);
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    TempData["message"] = $"\"{model.RoleName}\" created successfully";
                    return RedirectToAction("listroles", "admin");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ListRoles()
        {
            var role = roleManager.Roles;
            return View(role);
        }
        [HttpGet]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with id {role.Id} cannot be found";
                return View("notFound");
            }
            var model = new EditRoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
            };
            foreach (var user in userManager.Users.ToList())
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id: {model.RoleId} cannot be found";
                return View("notFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    TempData["message"] = $"Role \"{role.Name}\" created successfully";
                    return RedirectToAction("listroles");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpPost]
        //[Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    ViewBag.ErrorMessage = $"Role with id {role.Id} cannot be found";
                    return View("NotFound");
                }
                else
                {
                    try
                    {
                        var result = await roleManager.DeleteAsync(role);
                        if (result.Succeeded)
                        {
                            TempData["message"] = $"Role with id \"{id}\" deleted successfully";
                            return RedirectToAction("listroles", "admin");
                        }
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    catch (DbUpdateException ex)
                    {
                        logger.LogError($"Error deleting role {ex}");

                        ViewBag.ErrorTitle = $"{role.Name} role is in use";
                        ViewBag.ErrorMessage = $"{role.Name} role cannot be deleted as there are users in this role. If you want to delete this role, please remove users from the role and then try to delete";
                        return View("Error");
                    }
                }
            }
            return View("listroles");
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id: {userId} cannot be found";
                return View("notFound");
            }
            var existingUserClaims = await userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel
            {
                UserId = userId
            };
            foreach (Claim claim in ClaimsStore.Claims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type
                };
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);
            }

            return View(model);
        }
        [HttpPost]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with id {model.UserId} cannot be found";
                return View("notFound");
            }
            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }
            result = await userManager.AddClaimsAsync(user, model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected user to claims");
                return View(model);
            }
            return RedirectToAction("editStaff", "user", new { Id = model.UserId });
        }
        [HttpGet]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.userId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"User not found";
                return View("notfound");
            }
            var model = new List<UserRoleViewModel>();
            foreach (var user in userManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }
            return View(model);
        }
        [HttpPost]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"User not found";
                return View("notfound");
            }
            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult identityResult = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, roleId)))
                {
                    identityResult = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected)
                {
                    identityResult = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (identityResult.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("editRole", new { Id = roleId });
                }
            }
            return RedirectToAction("editRole", new { Id = roleId });
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User not found";
                return View("notFound");
            }
            var model = new List<UserRolesViewModel>();
            foreach (var role in roleManager.Roles.ToList())
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User not found";
                return View("notFound");
            }
            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot remove user existing roles");
                return View(model);
            }
            result = await userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot add user to roles");
                return View(model);
            }
            return RedirectToAction("editstaff", "user", new { Id = userId });
        }
        [HttpGet]
        public async Task<IActionResult> GetLga(string state)
        {
            var lgaList = new List<SelectListItem>();
            lgaList = await _db.StateLgas.Where(x => x.State == state).Select(x => new SelectListItem() { Text = x.Lga, Value = x.Lga }).Distinct().ToListAsync(); //call repository
            return Json(lgaList);
        }
    }
}
