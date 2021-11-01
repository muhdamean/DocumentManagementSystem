using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class PdfViewModel
    {
        public string Orientation { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
