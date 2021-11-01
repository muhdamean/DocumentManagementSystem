using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class EditRoleViewModel
    {
        [Required]
        public string RoleId { get; set; }
        [Required(ErrorMessage = "Role name is required")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }

        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
    }
}
