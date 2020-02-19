using System;
using System.Collections.Generic;

namespace Site02.Models
{
    public partial class Comment
    {
        public int CommentId { get; set; }
        public int TopicId { get; set; }
        public int? ParentCommentId { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public int Score { get; set; }
    }
}
