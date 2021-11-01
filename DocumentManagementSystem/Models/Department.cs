using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string SubmittedBy { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }
    }
}
