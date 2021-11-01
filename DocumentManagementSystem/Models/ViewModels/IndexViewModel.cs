using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<DocumentViewModel> GetDocumentViewModels { get; set; }
        public IEnumerable<DepartmentViewModel> GetDepartmentViewModels { get; set; }
        public UserViewModel GetUserViewModels { get; set; }
        public long TotalComments { get; set; }
        public long PdfCount { get; set; }
        public long WordCount { get; set; }
        public long PptCount { get; set; }
        public long ExlxCount { get; set; }
        public long AllDocCount { get; set; }

        public IEnumerable<DocTypeCount> DocTypeCounts { get; set; }

    }
    public class DocTypeCount
    {
        public string DocType { get; set; }
        public int Count { get; set; }
    }
}
