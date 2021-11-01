using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class EmailConfirmationLinkViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }
    }
}
