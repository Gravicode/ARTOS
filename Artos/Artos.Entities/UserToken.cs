using System;
using System.Collections.Generic;
using System.Text;

namespace Artos.Entities
{
    public class UserToken
    {
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public DateTime ValidUntil { get; set; }

        public static UserToken GenerateToken(string username)
        {
            var token = new UserToken() { AccessToken = Guid.NewGuid().ToString().Replace("-",""), ValidUntil = DateTime.Now.AddDays(1), Username = username };
            return token;
        }
    }
}
