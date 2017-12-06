using System;
using System.Collections.Generic;
using System.Text;

namespace Artos.Entities
{
    public class UserProfile
    {
        public long Id { get; set; }
        public string  Username { get; set; }
        public string  Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string ProfilePic { get; set; }
        public List<EMoney> MyWallet { get; set; }
    }
}
