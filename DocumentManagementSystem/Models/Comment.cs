using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public long DocumentId { get; set; }
        public string UserId { get; set; }
        [Display(Name ="Comment")]
        public string UserComment { get; set; }
        public DateTime Date { get; set; }
        public bool IsUpdate { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser  ApplicationUser { get; set; }
        [ForeignKey("DocumentId")]
        public Document Document { get; set; }
    }
}
