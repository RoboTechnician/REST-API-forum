using System;
using System.Collections.Generic;

namespace Site02.Models
{
    public partial class CommentScore
    {
        public int CommentId { get; set; }
        public string Username { get; set; }
        public LikeData.Score Score { get; set; }
    }
}
