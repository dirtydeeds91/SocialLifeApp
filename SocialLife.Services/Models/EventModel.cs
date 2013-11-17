using SocialLife.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialLife.Services.Models
{
    public class EventModel
    {
        public int EventId { get; set; }

        public string Name { get; set; }

        public string CreatorName { get; set; }

        public string AvatarUrl { get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public List<UserModel> UsersList { get; set; }

        public string Status { get; set; }
    }
}