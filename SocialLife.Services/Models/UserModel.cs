using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialLife.Services.Models
{
    public class UserModel
    {
        //user details
        public int Id { get; set; }

        public string Username { get; set; }

        public string DisplayName { get; set; }

        public string AuthCode { get; set; }

        public string SessionKey { get; set; }
    }
}