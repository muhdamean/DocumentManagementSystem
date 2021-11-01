using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class EditStaffViewModel
    {
        public EditStaffViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }
        public string Id { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Name { get; set; }
        [MaxLength(15)]
        public string Gender { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [MaxLength(50)]
        public string State { get; set; }
        [MaxLength(100)]
        public string Lga { get; set; }
        public string InitialLga { get; set; }
        public string Address { get; set; }
        [DataType(DataType.Date)]
        public string Department { get; set; }
       
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Display(Name = "Passport")]
        public IFormFile PhotoPath { get; set; }
        [Display(Name = "Current Passport")]
        public string OldPhotoPath { get; set; }
        public List<string> Claims { get; set; }
        public IList<string> Roles { get; set; }
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
       
    }
}
