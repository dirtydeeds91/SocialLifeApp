using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialLife.Services.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
        public string Status { get; set; }
        public Nullable<bool> Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Mood { get; set; }
        public string About { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}