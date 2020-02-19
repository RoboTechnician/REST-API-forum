using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;

namespace Site02.Models
{
    public partial class User : PublicUser
    {
        public const int SALT_LENGTH = 15;
        public const int PASSWORD_LENGTH = 15;

        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

        

        public void CreatePassword()
        {
            var byteSalt = new byte[SALT_LENGTH];
            var rngCsp = new RNGCryptoServiceProvider();

            rngCsp.GetBytes(byteSalt);
            Salt = Convert.ToBase64String(byteSalt);

            var t = Convert.FromBase64String(Salt);

            Password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: Password,
                salt: byteSalt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: PASSWORD_LENGTH));

        }

        public bool ValidatePassword(string password)
        {
            var hashPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Convert.FromBase64String(Salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: PASSWORD_LENGTH));

            return Password == hashPassword;
        }

        public async Task Authenticate(HttpContext httpContext)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Username)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await AuthenticationHttpContextExtensions.SignInAsync(httpContext, new ClaimsPrincipal(id));
        }
    }
}
