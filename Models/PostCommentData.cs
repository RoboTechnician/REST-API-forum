using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site02.Models
{
    public class PostCommentData
    {
        public string TopicId { get; set; }
        public string ParentCommentId { get; set; }
        public string Text { get; set; }


        public bool CheckPostText(PostCommentData err)
        {
            if (Text == "")
            {
                err.Text = "Text is required";
                return false;
            }
            else if (Text.Length >= 1000)
            {
                err.Text = "Text is too long";
                return false;
            }

            return true;
        }

        public async Task<bool> CheckPostTopicAndParentId(PostCommentData err, ForumContext context)
        {
            if (!int.TryParse(TopicId, out int topicId))
            {
                err.TopicId = "Topic id is incorrect";
                return false;
            }
            else if (await context.GetTopicById(topicId) == null)
            {
                err.TopicId = "Topic doesn't exist";
                return false;
            }
            else if (ParentCommentId != null)
            {
                if (!int.TryParse(ParentCommentId, out int parentCommentId))
                {
                    err.ParentCommentId = "Parent comment id is incorrect";
                    return false;
                }
                else if (await context.GetCommentInTopic(topicId, parentCommentId) == null)
                {
                    err.ParentCommentId = "Comment doesn't exist or doesn't belong to topic";
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> CheckPostData(PostCommentData err, ForumContext context)
        {
            var checkText = CheckPostText(err);
            var checkTopicAndParentId = await CheckPostTopicAndParentId(err, context);

            return checkTopicAndParentId && checkText;
        }
    }
}
