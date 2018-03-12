using System;
using System.Collections.Generic;

namespace Comments.ViewModels
{
    public class CommentsAndQuestions
    {
        public CommentsAndQuestions(IEnumerable<Comment> comments, IEnumerable<Question> questions)
        {
            Comments = comments;
            Questions = questions;
        }
        
        public IEnumerable<ViewModels.Comment> Comments { get; private set; }

        public IEnumerable<ViewModels.Question> Questions { get; private set; }
    }
}