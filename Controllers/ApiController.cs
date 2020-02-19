using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Site02.Models;

namespace Site02.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly ForumContext _context;

        public ApiController(ForumContext context)
        {
            _context = context;
        }

        // GET api/main
        [HttpGet("topics/new/{page?}")]
        public async Task<ActionResult<IEnumerable<Topic>>> GetNewTopics(int page = 1)
        {
            return await _context.GetNewTopics(page);
        }

        // GET api/main
        [HttpGet("topics/best/{page?}")]
        public async Task<ActionResult<IEnumerable<Topic>>> GetBestTopics(int page = 1)
        {
            return await _context.GetBestTopics(page);
        }

        [HttpGet("topics/fresh/{page?}")]
        public async Task<ActionResult<IEnumerable<Topic>>> GetFreshTopics([FromQuery] string date, int page = 1)
        {
            DateTime dateTime;
            dateTime = DateTime.TryParse(date, out dateTime) ? dateTime : DateTime.Now;
            //date = date == null ? DateTime.Now : date;
            return await _context.GetFreshTopics(page, dateTime);
        }

        [HttpGet("topic/comments/{id}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetTopicComments(int id)
        {
            return await _context.GetTopicComments(id);
        }

        [HttpGet("user/{username}")]
        public async Task<ActionResult<PublicUser>> GetPublicUser(string username)
        {
            var user = await _context.GetUserByUsername(username);
            return new PublicUser()
            {
                Username = user.Username,
                RegDate = user.RegDate
            };
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegData data)
        {
            var err = new RegData();

            if (await data.CheckRegData(err, _context))
            {
                var user = new User()
                {
                    Username = data.Username,
                    Email = data.Email,
                    Password = data.Password
                };

                user.CreatePassword();
                user.RegDate = DateTime.Now;
                await _context.AddUser(user);

                await user.Authenticate(HttpContext);

                return CreatedAtAction(nameof(GetPublicUser), new { user.Username }, new PublicUser()
                {
                    Username = user.Username,
                    RegDate = user.RegDate
                });
            }

            return BadRequest(err);
        }

        [HttpPost("login")]
        public async Task<ActionResult<PublicUser>> Login(LoginData data)
        {
            var user = await data.CheckLogin(_context);

            if (user != null)
            {
                await user.Authenticate(HttpContext);
                return CreatedAtAction(nameof(GetPublicUser), new { user.Username }, new PublicUser()
                {
                    Username = user.Username,
                    RegDate = user.RegDate
                });
            }

            return BadRequest("Login or Password are incorrect");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpPost("topic")]
        [Authorize]
        public async Task<IActionResult> PostTopic(PostTopicData data)
        {
            var err = new PostTopicData();
            if (data.CheckPostData(err))
            {
                var topic = new Topic()
                {
                    Username = User.Identity.Name,
                    Text = data.Text,
                    Theme = data.Theme,
                    Date = DateTime.Now,
                    Score = 0
                };
                await _context.AddTopic(topic);

                return Ok();
            }

            return BadRequest(err);
        }

        [HttpPost("comment")]
        [Authorize]
        public async Task<IActionResult> PostComment(PostCommentData data)
        {
            var err = new PostCommentData();
            if (await data.CheckPostData(err, _context))
            {
                var comment = new Comment()
                {
                    TopicId = int.Parse(data.TopicId),
                    ParentCommentId = data.ParentCommentId == null ? null : (int?)int.Parse(data.ParentCommentId),
                    Username = User.Identity.Name,
                    Text = data.Text,
                    Date = DateTime.Now,
                    Score = 0
                };
                await _context.AddComment(comment);

                return Ok();
            }

            return BadRequest(err);
        }

        [HttpPost("topic/like")]
        [Authorize]
        public async Task<IActionResult> LikeTopic(LikeData id)
        {
            var err = await id.LikeTopic(User.Identity.Name, _context);
            if (err != null)
                return BadRequest(err);

            return Ok();
        }

        [HttpPost("topic/dislike")]
        [Authorize]
        public async Task<IActionResult> DislikeTopic(LikeData id)
        {
            var err = await id.DislikeTopic(User.Identity.Name, _context);
            if (err != null)
                return BadRequest(err);

            return Ok();
        }

        [HttpPost("comment/like")]
        [Authorize]
        public async Task<IActionResult> LikeComment(LikeData id)
        {
            var err = await id.LikeComment(User.Identity.Name, _context);
            if (err != null)
                return BadRequest(err);

            return Ok();
        }

        [HttpPost("comment/dislike")]
        [Authorize]
        public async Task<IActionResult> DislikeComment(LikeData id)
        {
            var err = await id.DislikeComment(User.Identity.Name, _context);
            if (err != null)
                return BadRequest(err);

            return Ok();
        }



        //// GET api/main/topic/{id}
        //[HttpGet("topic/{id}")]
        //public async Task<ActionResult<IEnumerable<MainPost>>> GetTopic(int id)
        //{
        //    var posts = await _context.GetTopic(id);

        //    if (posts)
        //    return await _context.GetTopic(id);
        //}

        //// GET api/main/post/{id}
        //[HttpGet("post/{id}")]
        //public async Task<ActionResult<MainPost>> GetPost(int id)
        //{
        //    return await _context.GetPost(id);
        //}

        //// POST api/main
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
