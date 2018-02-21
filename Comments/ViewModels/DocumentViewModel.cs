using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Comments.Models;

namespace Comments.ViewModels
{
    public class DocumentViewModel
    {
        public DocumentViewModel(IEnumerable<Comment> comments)
        {
            Comments = comments;
        }

        public IEnumerable<Comment> Comments { get; set; }
    }
}
