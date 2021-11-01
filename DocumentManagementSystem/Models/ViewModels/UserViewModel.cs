using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class UserViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Department { get; set; }
        public DateTime DOB { get; set; }
        public string State { get; set; }
        public string Lga { get; set; }
        public string Address { get; set; }
        public string PhotoPath { get; set; }
        public bool IsActive { get; set; }
    }
}
