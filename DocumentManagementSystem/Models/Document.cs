using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models
{
    public class Document
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Status { get; set; }
        public bool IsNew { get; set; }
        [Display(Name ="Comment")]
        public string UploadComment { get; set; }
        public string UserId { get; set; }
        public string UploadId { get; set; }
        public string Update_UploadId { get; set; }
        public string Department { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
      
    }
}
