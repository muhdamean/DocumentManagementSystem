using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Models.ViewModels
{
    public class DocumentCommentViewModel
    {
        public IEnumerable<DocumentViewModel> GetDocumentsViewModel { get; set; }
        public IEnumerable<CommentViewModel> GetCommentsViewModel { get; set; }
        public string SelectedDepartment { get; set; }
    }
}
