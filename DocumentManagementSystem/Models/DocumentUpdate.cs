using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models
{
    public class DocumentUpdate
    {
        public long Id { get; set; }
        public long DocumentId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string Comment { get; set; }
        public string UpdatedBy { get; set; }
        public string Update_UploadId { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
