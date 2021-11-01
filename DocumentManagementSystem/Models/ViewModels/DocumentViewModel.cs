using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class DocumentViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        
        public string Status { get; set; }
        public bool IsNew { get; set; }
        public string UploadId { get; set; }
        public string Update_UploadId { get; set; }
        public string UploadedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Display(Name ="Choose Document ")]
        public List<IFormFile> DocumentUpload { get; set; }
        [Display(Name = "Name ")]
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Passport { get; set; }
        [Display(Name = "Comment")]
        public string UploadComment { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
    }
}
