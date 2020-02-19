using System;
using System.Collections.Generic;

namespace Site02.Models
{
    public partial class Topic
    {
        public int TopicId { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public string Theme { get; set; }
        public string Text { get; set; }
        public int Score { get; set; }
    }
}
