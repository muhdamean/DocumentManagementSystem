using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(250)]
        public string Name { get; set; }
        [MaxLength(10)]
        public string Gender { get; set; }
        [MaxLength(100)]
        public string Department { get; set; }
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        public string State { get; set; }
        public string Lga { get; set; }
        public string Address { get; set; }
        public string PhotoPath { get; set; }
        public bool IsActive { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        //[Display(Name = "Name")]
        //public string FullName
        //{
        //    get { return FirstName + " " + MiddleName + " " + LastName; }
        //}
    }
}
