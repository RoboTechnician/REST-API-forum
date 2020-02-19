using System;
using System.Collections.Generic;

namespace Site02.Models
{
    public partial class TopicScore
    {
        public int TopicId { get; set; }
        public string Username { get; set; }
        public LikeData.Score Score { get; set; }
    }
}
