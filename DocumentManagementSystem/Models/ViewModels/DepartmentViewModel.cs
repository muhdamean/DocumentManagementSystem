using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class DepartmentViewModel
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }
        public string SubmittedBy { get; set; }
    }
}
