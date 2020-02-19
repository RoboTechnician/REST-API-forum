using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Site02.Models
{
    public class RegData
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CheckRegEmail(RegData err, ForumContext context)
        {
            if (Email == "")
            {
                err.Email = "Email is required";
                return false;
            }
            else if (Email.Length >= 50)
            {
                err.Email = "Email is too long";
                return false;
            }
            else if (!IsValidEmail(Email))
            {
                err.Email = "Email is invalid";
                return false;
            }
            else if ((await context.GetUserByEmail(Email)) != null)
            {
                err.Email = "Email is already in use";
                return false;
            }

            return true;
        }

        public async Task<bool> CheckRegUsername(RegData err, ForumContext context)
        {
            if (Username == "")
            {
                err.Username = "Username is required";
                return false;
            }
            else if (Username.Length >= 20)
            {
                err.Username = "Username is too long";
                return false;
            }
            else if ((await context.GetUserByUsername(Username)) != null)
            {
                err.Username = "Username is already taken";
                return false;
            }

            return true;
        }

        public bool CheckRegPassword(RegData err)
        {
            if (Password == "")
            {
                err.Password = "Password is required";
                return false;
            }
            else if (Password.Length >= 50)
            {
                err.Password = "Password is too long";
                return false;
            }
            else if (Password.Length < 6)
            {
                err.Password = "Password is too short";
                return false;
            }

            return true;
        }

        public async Task<bool> CheckRegData(RegData err, ForumContext context)
        {
            var checkUsername = await CheckRegUsername(err, context);
            var checkEmail = await CheckRegEmail(err, context);
            var checkPassword = CheckRegPassword(err);

            return checkUsername && checkEmail && checkPassword;
        }
    }
}
