﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLife.Models
{
    [Table("Profiles")]
    public partial class Profile
    {
        //Database fields
        [Key]
        public int UserId { get; set; }

        public int StatusId { get; set; }

        public string Avatar { get; set; }

        public string FriendsList { get; set; }

        public Nullable<bool> Gender { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        [MaxLength(20)]
        public string Mood { get; set; }

        public string About { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? BirthDate { get; set; }

        //Database relationships
        public virtual Status Status { get; set; }

        [Required]
        public virtual User User { get; set; }
    }
}
