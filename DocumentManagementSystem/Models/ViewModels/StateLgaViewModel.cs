using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class StateLgaViewModel
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string State { get; set; }
        [MaxLength(100)]
        public string Lga { get; set; }
        public IEnumerable<string> StateList { get; set; }
        public IEnumerable<string> LgaList { get; set; }
    }
}
