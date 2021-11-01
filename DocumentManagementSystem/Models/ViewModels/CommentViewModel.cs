using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class CommentViewModel
    {
        public long Id { get; set; }
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Comment")]
        public string UserComment { get; set; }
        public DateTime Date { get; set; }
        public bool IsUpdate { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
    }
}
