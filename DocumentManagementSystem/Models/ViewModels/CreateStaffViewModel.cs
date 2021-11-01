using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class CreateStaffViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "IsEmailInUse", controller: "Account", ErrorMessage = "Email already in use")]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [MaxLength(15)]
        public string Gender { get; set; }
        [Required]
        [Display(Name ="Phone")]
        public string PhoneNumber { get; set; }
        [MaxLength(50)]
        public string State { get; set; }
        [MaxLength(100)]
        [Display(Name="LGA")]
        public string Lga { get; set; }
        public string Address { get; set; }        
        public string Department { get; set; }
       
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Display(Name = "Passport")]
        public IFormFile PhotoPath { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "New password and confirm password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
