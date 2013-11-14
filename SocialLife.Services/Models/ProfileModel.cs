using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialLife.Services.Models
{
    public class ProfileModel
    {
        public int UserId { get; set; }

        public string AuthCode { get; set; }

        public string DisplayName { get; set; }

        //profile details
        public string Avatar { get; set; }

        public string Status { get; set; }

        public Nullable<bool> Gender { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Mood { get; set; }

        public string About { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? BirthDate { get; set; }

        public List<UserModel> FriendsList { get; set; }

        public string LastLongitute { get; set; }

        public string LastLatitude { get; set; }

        public DateTime LastLocationDate { get; set; }
    }
}