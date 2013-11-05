using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLife.Models
{
    [Table("Users")]
    public class User
    {
        //Constructor
        public User()
        {
            this.Locations = new HashSet<Location>();
            this.MessageSender = new HashSet<Message>();
            this.MessageReceiver = new HashSet<Message>();
            this.Events = new HashSet<Event>();
        }

        //Database fields
        [Key]
        public int UserId { get; set; }

        [Required, MinLength(6), MaxLength(30), StringLength(30, MinimumLength = 6)]
        public string Username { get; set; }

        [Required, MinLength(6), MaxLength(30), StringLength(30, MinimumLength = 6)]
        public string DisplayName { get; set; }

        [Required, MinLength(40), MaxLength(40), StringLength(40, MinimumLength=40)]
        public string AuthCode { get; set; }

        public string SessionKey { get; set; }

        //Database relationships
        public virtual Profile Profile { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Location> Locations { get; set; }

        [InverseProperty("Sender")]
        public virtual ICollection<Message> MessageSender { get; set; }

        [InverseProperty("Receiver")]
        public virtual ICollection<Message> MessageReceiver { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
