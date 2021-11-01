using DocumentManagementSystem.Models;
using DocumentManagementSystem.Models.ViewModels;
using DocumentManagementSystem.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Controllers
{
    public class DocumentController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DocumentController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this._db = appDbContext;
            this.webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Index()
        {
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
            var comments = _db.Comments.Include(x => x.ApplicationUser).ToList();
            List<CommentViewModel> commentList = new List<CommentViewModel>();
            foreach (var comment in comments)
            {
                CommentViewModel commentViewModel = new CommentViewModel
                {
                    Id = comment.Id,
                    UserComment = comment.UserComment,
                    UserId = comment.UserId,
                    DocumentId = comment.DocumentId,
                    Date = comment.Date,
                    UserName = comment.ApplicationUser.Name,
                    UserEmail = comment.ApplicationUser.Email
                };
                commentList.Add(commentViewModel);
            }
            DocumentCommentViewModel documentCommentViewModel = new DocumentCommentViewModel();
            documentCommentViewModel.GetDocumentsViewModel = documentList;
            documentCommentViewModel.GetCommentsViewModel = commentList;
            return View(documentCommentViewModel);
        }
        [HttpPost]
        public IActionResult Index(DocumentViewModel model)
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Documents()
        {
            var document = _db.Documents.AsNoTracking().ToList().OrderBy(x => x.Name);
            List<DocumentViewModel> documentList = new List<DocumentViewModel>();
            foreach (var item in document)
            {
                var user =await userManager.FindByIdAsync(item.UserId);
                DocumentViewModel documentViewModel = new DocumentViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Size=item.Size,
                    Type=item.Type,
                    IsNew=item.IsNew,
                    UploadId=item.UploadId,
                    Status = item.Status,
                    Date = item.Date.Date,
                    UploadedBy = user.Email
                };
                documentList.Add(documentViewModel);
            }
            return View(documentList);
        }
        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            //department list
            var DeptList = new List<SelectListItem>();
            DeptList = await _db.Departments.Select(x => new SelectListItem() { Text = x.Name, Value = x.Name }).Distinct().OrderBy(x => x.Value).ToListAsync();
            DeptList.Insert(0, new SelectListItem() { Text = "General", Value = "General" });
            ViewBag.Department = DeptList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(DocumentViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                string appendDate = DateTime.Now.ToString("yyyyMMddhhmmsss");
                if (model.DocumentUpload != null && model.DocumentUpload.Count > 0)
                {
                    var user = await userManager.FindByEmailAsync(User.Identity.Name);
                    if (user==null)
                    {
                        TempData["message"] = "Oops!,Kindly login again";
                        return View("index");
                    }
                    string userId = user.Id;
                    foreach (IFormFile photo in model.DocumentUpload)
                    {
                        string filename = string.Empty, ext = string.Empty, type=string.Empty;
                        long size=0;
                        size = photo.Length;
                        size = size / 1024;
                        filename = Path.GetFileNameWithoutExtension(photo.FileName);
                        ext = Path.GetExtension(photo.FileName);
                        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "documents");
                        uniqueFileName = filename + "_" + appendDate + ext;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        //get extension type
                        type= GetDocumentExtensionType(ext);

                        var newDocument = new Document
                        {
                            Name = filename,
                            Type = type,
                            IsNew = true,
                            UploadId = uniqueFileName,
                            Size = size.ToString(),
                            UserId = userId,
                            Status = "New Document",
                            Department=model.Department,
                            Date = DateTime.Now.Date,
                            UploadComment=model.UploadComment
                        };
                        await _db.Documents.AddAsync(newDocument);
                        await _db.SaveChangesAsync();

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await photo.CopyToAsync(fileStream);
                        }
                    }
                    return RedirectToAction("index");
                }
            }
            return View();
        }
        [HttpGet]
        public IActionResult Type(string type)
        {
            TempData["DocType"] = type.ToUpper().Trim();
            var document = _db.Documents.Include(x => x.ApplicationUser).ToList().Where(x=>x.Type==type.ToUpper().Trim()).OrderByDescending(x => x.Date);
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
            var comments = _db.Comments.Include(x => x.ApplicationUser).ToList();
            List<CommentViewModel> commentList = new List<CommentViewModel>();
            foreach (var comment in comments)
            {
                CommentViewModel commentViewModel = new CommentViewModel
                {
                    Id = comment.Id,
                    UserComment = comment.UserComment,
                    UserId = comment.UserId,
                    DocumentId = comment.DocumentId,
                    Date = comment.Date,
                    UserName = comment.ApplicationUser.Name,
                    UserEmail = comment.ApplicationUser.Email
                };
                commentList.Add(commentViewModel);
            }
            DocumentCommentViewModel documentCommentViewModel = new DocumentCommentViewModel();
            documentCommentViewModel.GetDocumentsViewModel = documentList;
            documentCommentViewModel.GetCommentsViewModel = commentList;
            return View(documentCommentViewModel);
        }
        [HttpPost]
        public IActionResult Type(DocumentViewModel model)
        {
            return View();
        }
        private static string GetDocumentExtensionType(string type)
        {
            string DocType = string.Empty;
            switch (type.ToUpper())
            {
                case ".PDF":
                    return "PDF";
                case ".PPT":
                    return "PPT";
                case ".XLSX":
                case ".XLS":
                    return "XLSX";
                case ".DOCX":
                case ".DOC":
                    return "DOCX";
                default:
                    return "Other";
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> AddComment(long id)
        {
            var role = await _db.Documents.FirstOrDefaultAsync(x=>x.Id==id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Document not found";
                return View("notFound");
            }
            CommentViewModel commentViewModel = new CommentViewModel
            {
                DocumentId = id,
            };
            return View(commentViewModel);
        }
        [HttpPost]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> AddComment(CommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doc = await _db.Documents.FirstOrDefaultAsync(x => x.Id == model.DocumentId);
                if (doc == null)
                {
                    ViewBag.ErrorMessage = "Document not found";
                    return View("notFound");
                }
                else
                {
                    var user = await userManager.FindByEmailAsync(User.Identity.Name);
                    string userId = user.Id;
                    var comment = new Comment
                    {
                        UserComment = model.UserComment,
                        Date = DateTime.Now,
                        DocumentId = model.DocumentId,
                        UserId =userId
                    };
                    var result = await _db.Comments.AddAsync(comment);
                    await _db.SaveChangesAsync();
                    if (result != null)
                    {
                        TempData["message"] = "Comment added successfully";
                        return RedirectToAction("index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "error posting comment");
                    }
                    return RedirectToAction("index");
                }
                
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UpdateDocumentStatus(long id)
        {
            var dept = await _db.Documents.FirstOrDefaultAsync(x => x.Id == id);
            if (dept == null)
            {
                ViewBag.ErrorMessage = $"Document not found";
                return View("notFound");
            }
            var status = new List<SelectListItem>();
            status.Insert(0, new SelectListItem() { Text = "---Select Status---", Value = "0" });
            status.Insert(1, new SelectListItem() { Text = "New Document", Value = "New Document" });
            status.Insert(2, new SelectListItem() { Text = "Under Review", Value = "Under Review" });
            status.Insert(3, new SelectListItem() { Text = "Approved", Value = "Approved" });
            status.Insert(3, new SelectListItem() { Text = "Rejected", Value = "Rejected" });
            ViewBag.Status = status;
            //department list
            var DeptList = new List<SelectListItem>();
            DeptList = await _db.Departments.Select(x => new SelectListItem() { Text = x.Name, Value = x.Name }).Distinct().OrderBy(x => x.Value).ToListAsync();
            DeptList.Insert(0, new SelectListItem() { Text = "---Select Department---", Value = string.Empty });
            ViewBag.Department = DeptList;
            DocumentViewModel docViewModel = new DocumentViewModel
            {
                Id = dept.Id,
                Department=dept.Department,
                Status=dept.Status
            };
            return View(docViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateDocumentStatus(DocumentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doc = await _db.Documents.FirstOrDefaultAsync(x => x.Id == model.Id);
                if (doc == null)
                {
                    ViewBag.ErrorMessage = "Document not found";
                    return View("notFound");
                }
                else
                {
                    doc.Status = model.Status;
                    doc.Department = model.Department;
                    var result = await _db.Documents.AddAsync(doc);
                    await _db.SaveChangesAsync();
                    if (result != null)
                    {
                        TempData["message"] = "Document updated successfully";
                        return RedirectToAction("documents");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "error updating status");
                    }
                }
                return View("index");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DeptDocument()
        {
            var user =await userManager.FindByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                TempData["message"] = "Oops!,Kindly login again";
                return View("DeptDocument");
            }
            else if(user!=null)
            {
                var document = _db.Documents.Include(x => x.ApplicationUser).ToList().Where(x => x.Department == user.Department).OrderBy(x => x.Name);
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
                var comments = await _db.Comments.Include(x => x.ApplicationUser).ToListAsync();
                List<CommentViewModel> commentList = new List<CommentViewModel>();
                foreach (var comment in comments)
                {
                    CommentViewModel commentViewModel = new CommentViewModel
                    {
                        Id = comment.Id,
                        UserComment = comment.UserComment,
                        UserId = comment.UserId,
                        DocumentId = comment.DocumentId,
                        Date = comment.Date,
                        UserName = comment.ApplicationUser.Name,
                        UserEmail = comment.ApplicationUser.Email
                    };
                    commentList.Add(commentViewModel);
                }
                DocumentCommentViewModel documentCommentViewModel = new DocumentCommentViewModel();
                documentCommentViewModel.GetDocumentsViewModel = documentList;
                documentCommentViewModel.GetCommentsViewModel = commentList;
                return View(documentCommentViewModel);
            }
            else
            {
                TempData["message"] = "Oops!,You're not yet assign a department";
                return View();
            }

        }
        [HttpPost]
        public IActionResult DeptDocument(DocumentCommentViewModel model)
        {
            return View();
        }
        [HttpGet]
        public IActionResult DownloadFile(long? id)
        {
            // Since this is just and example, I am using a local file located inside wwwroot
            // Afterwards file is converted into a stream
            if (id==null)
            {
                TempData["message"] = $"file not found";
                return View("documents"); 
            }
            var uploadId = _db.Documents.FirstOrDefault(x => x.Id == id);
            if (uploadId==null)
            {
                TempData["message"] = $"file with id \"{uploadId}\" not found";
                return View("documents");
            }
            string filename = string.Empty;
            filename= uploadId.UploadId;
            var path = Path.Combine(webHostEnvironment.WebRootPath,"documents", filename);
            var fs = new FileStream(path, FileMode.Open);

            // Return the file. A byte array can also be used instead of a stream
            return File(fs, "application/octet-stream", filename);
        }
        private string ProcessUploadedFile(DocumentViewModel model)
        {
            string uniqueFileName = null;
            string appendDate = DateTime.Now.ToString("yyyyMMddhhmmsss");
            if (model.DocumentUpload != null && model.DocumentUpload.Count > 0)
            {
                foreach (IFormFile photo in model.DocumentUpload)
                {
                    
                    string filename = string.Empty, ext = string.Empty;
                    filename= Path.GetFileNameWithoutExtension(photo.FileName);
                    ext = Path.GetExtension(photo.FileName);
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    uniqueFileName = filename+"_"+appendDate+"_"+ext;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }

                }

            }
            
            //string uniqueFileName = null;
            //if (model.Photos != null)
            //{
            //    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
            //    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            //    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            //    photo.CopyTo(new FileStream(filePath, FileMode.Create));
            //}
            return uniqueFileName;
        }
    }
}
