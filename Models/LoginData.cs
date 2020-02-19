using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Site02.Models
{
    public class LoginData
    {
        public string UsernameOrEmail { get; set; }
        public string Password { get; set; }

        public async Task<User> CheckLogin(ForumContext context)
        {
            var user = await context.GetUserByUsernameOrEmail(UsernameOrEmail);

            return user != null && user.ValidatePassword(Password) ? user : null;
        }
    }
}
