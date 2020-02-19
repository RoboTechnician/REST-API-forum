using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site02.Models
{
    public class LikeData
    {
        public string Id { get; set; }

        public enum Score
        {
            Like,
            Dislike
        }

        public async Task<string> LikeTopic(string username, ForumContext context)
        {
            if (!int.TryParse(Id, out int id))
                return "Id is invalid";
            else if (await context.GetTopicById(id) == null)
                return "Topic doesn't exist";

            var like = await context.GetTopicScore(id, username);
            if (like == null)
            {
                await context.LikeTopic(id, username);
                return null;
            }
            else if (like.Score == Score.Dislike)
            {
                await context.DeleteTopicDislike(id, username);
                return null;
            }
            else
                return "User is already like topic";
        }

        public async Task<string> DislikeTopic(string username, ForumContext context)
        {
            if (!int.TryParse(Id, out int id))
                return "Id is invalid";
            else if (await context.GetTopicById(id) == null)
                return "Topic doesn't exist";

            var like = await context.GetTopicScore(id, username);
            if (like == null)
            {
                await context.DislikeTopic(id, username);
                return null;
            }
            else if (like.Score == Score.Like)
            {
                await context.DeleteTopicLike(id, username);
                return null;
            }
            else
                return "User is already dislike topic";
        }

        public async Task<string> LikeComment(string username, ForumContext context)
        {
            if (!int.TryParse(Id, out int id))
                return "Id is invalid";
            else if (await context.GetCommentById(id) == null)
                return "Comment doesn't exist";

            var like = await context.GetCommentScore(id, username);
            if (like == null)
            {
                await context.LikeComment(id, username);
                return null;
            }
            else if (like.Score == Score.Dislike)
            {
                await context.DeleteCommentDislike(id, username);
                return null;
            }
            else
                return "User is already like comment";
        }

        public async Task<string> DislikeComment(string username, ForumContext context)
        {
            if (!int.TryParse(Id, out int id))
                return "Id is invalid";
            else if (await context.GetCommentById(id) == null)
                return "Comment doesn't exist";

            var like = await context.GetCommentScore(id, username);
            if (like == null)
            {
                await context.DislikeComment(id, username);
                return null;
            }
            else if (like.Score == Score.Like)
            {
                await context.DeleteCommentLike(id, username);
                return null;
            }
            else
                return "User is already dislike comment";
        }
    }
}
