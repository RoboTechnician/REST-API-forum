using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Site02.Models
{
    public partial class ForumContext
    {
        public async Task<List<Topic>> GetNewTopics(int page)
        {
            return await Topics.FromSql($"SELECT * FROM topics ORDER BY date DESC LIMIT {10 * (page - 1)}, 10;").ToListAsync();
        }

        public async Task<List<Topic>> GetBestTopics(int page)
        {
            return await Topics.FromSql($"SELECT * FROM topics ORDER BY score DESC LIMIT {10 * (page - 1)}, 10;").ToListAsync();
        }

        public async Task<List<Topic>> GetFreshTopics(int page, DateTime date)
        {
            return await Topics.FromSql(
                "SELECT * FROM topics WHERE date <= {0} AND date > {1} ORDER BY score DESC LIMIT {2}, 10;", date, date.AddDays(-1), 10 * (page - 1)).ToListAsync();
        }

        public async Task<Topic> GetTopicById(int topicId)
        {
            return await Topics.FindAsync(topicId);
        }

        public async Task<Comment> GetCommentById(int commentId)
        {
            return await Comments.FindAsync(commentId);
        }

        public async Task<Comment> GetCommentInTopic(int topicId, int commentId)
        {
            return await Comments.FirstOrDefaultAsync(c => c.CommentId == commentId && c.TopicId == topicId);
        }

        public async Task<List<Comment>> GetTopicComments(int topicId)
        {
            return await Comments.Where(c => c.TopicId == topicId).ToListAsync();
            //return await Comments.FromSql($"SELECT * FROM comments WHERE topic_id = {topicId};").ToListAsync();
        }

        public async Task<TopicScore> GetTopicScore(int topicId, string username)
        {
            return await TopicScores.FirstOrDefaultAsync(ts => ts.TopicId == topicId && ts.Username == username);
        }

        public async Task<CommentScore> GetCommentScore(int commentId, string username)
        {
            return await CommentScores.FirstOrDefaultAsync(cs => cs.CommentId == commentId && cs.Username == username);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await Users.FindAsync(username);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByUsernameOrEmail(string usernameOrEmail)
        {
            return await Users.FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);
            //return await Users.FromSql($"SELECT * FROM users WHERE username = '{usernameOrEmail}' OR email = '{usernameOrEmail}' LIMIT 1;").FirstOrDefaultAsync();
        }

        public async Task<bool> AddUser(User user)
        {
            Users.Add(user);
            return await SaveChangesAsync() != 0;
        }

        public async Task<bool> AddTopic(Topic topic)
        {
            Topics.Add(topic);
            return await SaveChangesAsync() != 0;
        }

        public async Task<bool> AddComment(Comment comment)
        {
            Comments.Add(comment);
            return await SaveChangesAsync() != 0;
        }

        public async Task<bool> LikeTopic(int topicId, string username)
        {
            int result = 0;
            result += await ChangeTopicScore(topicId, 1);
            result += await AddTopicScore(topicId, username, LikeData.Score.Like);

            return result != 0;
        }

        public async Task<bool> DeleteTopicLike(int topicId, string username)
        {
            int result = 0;
            result += await ChangeTopicScore(topicId, -1);
            result += await DeleteTopicScore(topicId, username);

            return result != 0;
        }

        public async Task<bool> DislikeTopic(int topicId, string username)
        {
            int result = 0;
            result += await ChangeTopicScore(topicId, -1);
            result += await AddTopicScore(topicId, username, LikeData.Score.Dislike);

            return result != 0;
        }

        public async Task<bool> DeleteTopicDislike(int topicId, string username)
        {
            int result = 0;
            result += await ChangeTopicScore(topicId, 1);
            result += await DeleteTopicScore(topicId, username);

            return result != 0;
        }

        private async Task<int> ChangeTopicScore(int topicId, int score)
        {
            return await Database.ExecuteSqlCommandAsync("UPDATE topics SET score=score+{0} WHERE topic_id = {1};", score, topicId);
        }

        private async Task<int> AddTopicScore(int topicId, string username, LikeData.Score score)
        {
            TopicScores.Add(new TopicScore()
            {
                TopicId = topicId,
                Username = username,
                Score = score
            });
            return await SaveChangesAsync();
        }

        private async Task<int> DeleteTopicScore(int topicId, string username)
        {
            TopicScores.Remove(TopicScores.First(ts => ts.TopicId == topicId && ts.Username == username));
            return await SaveChangesAsync();
        }

        public async Task<bool> LikeComment(int commentId, string username)
        {
            int result = 0;
            result += await ChangeCommentScore(commentId, 1);
            result += await AddCommentScore(commentId, username, LikeData.Score.Like);

            return result != 0;
        }

        public async Task<bool> DeleteCommentLike(int commentId, string username)
        {
            int result = 0;
            result += await ChangeCommentScore(commentId, -1);
            result += await DeleteCommentScore(commentId, username);

            return result != 0;
        }

        public async Task<bool> DislikeComment(int commentId, string username)
        {
            int result = 0;
            result += await ChangeCommentScore(commentId, -1);
            result += await AddCommentScore(commentId, username, LikeData.Score.Dislike);

            return result != 0;
        }

        public async Task<bool> DeleteCommentDislike(int commentId, string username)
        {
            int result = 0;
            result += await ChangeCommentScore(commentId, 1);
            result += await DeleteCommentScore(commentId, username);

            return result != 0;
        }

        private async Task<int> ChangeCommentScore(int commentId, int score)
        {
            return await Database.ExecuteSqlCommandAsync("UPDATE comments SET score = score + {0} WHERE comment_id = {1};", score, commentId);
        }

        private async Task<int> AddCommentScore(int commentId, string username, LikeData.Score score)
        {
            CommentScores.Add(new CommentScore()
            {
                CommentId = commentId,
                Username = username,
                Score = score
            });
            return await SaveChangesAsync();
        }

        private async Task<int> DeleteCommentScore(int commentId, string username)
        {
            CommentScores.Remove(CommentScores.First(cs => cs.CommentId == commentId && cs.Username == username));
            return await SaveChangesAsync();
        }
    }
}
