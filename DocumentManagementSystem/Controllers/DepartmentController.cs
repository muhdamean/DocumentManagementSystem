using DocumentManagementSystem.Models;
using DocumentManagementSystem.Models.ViewModels;
using DocumentManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DepartmentController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._db = appDbContext;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var department = _db.Departments.AsNoTracking().ToList().OrderBy(x => x.Name);
            List<DepartmentViewModel> departments = new List<DepartmentViewModel>();
            foreach (var item in department)
            {
                DepartmentViewModel departmentsViewModel = new DepartmentViewModel
                {
                    DepartmentId = item.Id,
                    Name = item.Name,
                    Status = item.IsActive,
                    DateCreated = item.DateCreated.Date,
                    SubmittedBy = item.SubmittedBy
                };
                departments.Add(departmentsViewModel);
            }
            
            return View(departments);
        }
        [HttpGet]
        [Authorize(Policy = "CreateRolePolicy")]
        public IActionResult CreateDepartment()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Policy = "CreateRolePolicy")]
        public async Task<IActionResult> CreateDepartment(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var department = new Department
                {
                    Name = model.Name,
                    IsActive = true,
                    SubmittedBy = User.Identity.Name,
                    DateCreated = DateTime.Now.Date
                };
                await _db.Departments.AddAsync(department);
                var result = await _db.SaveChangesAsync();
                if (result > 0)
                {
                    TempData["message"] = $"\"{model.Name}\" created successfully";
                    return RedirectToAction("index");
                }
                else
                {
                    DbUpdateException ex = new DbUpdateException();
                    ModelState.AddModelError(string.Empty, "Error saving record");
                }
            }
            return View(model);
        }
        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public  IActionResult EditDepartment(int id)
        {
            var dept = _db.Departments.FindAsync(id);
            if (dept.Result == null)
            {
                ViewBag.ErrorMessage = $"Record with Id: {id} cannot be found";
                return View("notfound");
            }
            else
            {
                DepartmentViewModel model = new DepartmentViewModel
                {
                    DepartmentId = dept.Result.Id,
                    Name = dept.Result.Name,
                    Status = dept.Result.IsActive,
                    DateCreated = dept.Result.DateCreated,
                    SubmittedBy = dept.Result.SubmittedBy
                };
                return View(model);
            }
            
        }
        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditDepartment(DepartmentViewModel model)
        {
            if (ModelState.IsValid)
            {

                var id = model.DepartmentId;
                var dept = await _db.Departments.FirstOrDefaultAsync(x => x.Id == id);
                if (dept == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid record Id details");
                }
                else
                {
                    dept.Name = model.Name;
                    dept.DateCreated = model.DateCreated;
                    dept.IsActive = true;
                    dept.SubmittedBy = User.Identity.Name;
                    _db.Departments.Update(dept);
                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        TempData["message"] = $"\"{model.Name}\" updated successfully";
                        return RedirectToAction("index");
                    }
                    else
                    {
                        DbUpdateException ex = new DbUpdateException();
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                };

            }

            return View(model);
        }
        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteDepartments(int id)
        {
            if (ModelState.IsValid)
            {
                var dept = await _db.Departments.FirstOrDefaultAsync(x => x.Id == id);
                if (dept == null)
                {
                    ViewBag.ErrorMessage = $"Record with id {id} cannot be found";
                    return View("notfound");
                }
                else
                {
                    string deptName = string.Empty;
                    deptName = dept.Name;
                    _db.Departments.Remove(dept);
                    var result = await _db.SaveChangesAsync();
                    if (result > 0)
                    {
                        TempData["message"] = $"\"{deptName}\" deleted successfully";
                        return RedirectToAction("index");
                    }
                    else
                    {
                        DbUpdateException ex = new DbUpdateException();
                        ModelState.AddModelError("", ex.Message);
                    }
                }
            }
            return View("index");
        }
    }
}
