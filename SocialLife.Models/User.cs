using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialLife.Models
{
    public class User
    {
        //Constructor
        public User()
        {
            this.Locations = new HashSet<Location>();
            //this.SentMessages = new HashSet<Message>();
            //this.ReceivedMessages = new HashSet<Message>();
        }

        //Database fields
        [Key]
        public int UserId { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(30)]
        public string Username { get; set; }

        [MinLength(40)]
        [MaxLength(40)]
        [Required]
        public string Password { get; set; }

        //Database relationships
        public virtual Profile Profile { get; set; }

        public virtual ICollection<Location> Locations { get; set; }


        //[ForeignKey("SenderId")]
        //public virtual ICollection<Message> SentMessages { get; set; }

        //[ForeignKey("ReceiverId")]
        //public virtual ICollection<Message> ReceivedMessages { get; set; }
    }
}
