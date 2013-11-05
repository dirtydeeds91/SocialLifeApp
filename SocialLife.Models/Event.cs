using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SocialLife.Models
{
    [Table("Events")]
    public partial class Event
    {
        public Event()
        {
            this.Messages = new HashSet<Message>();
            this.Users = new HashSet<User>();
        }

        [Key]
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; }

        [Required]
        public string EventContent { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        //[ForeignKey("Location")]
        public int LocationId { get; set; }

        public int StatusId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location EventLocation { get; set; }

        [ForeignKey("StatusId")]
        public virtual Status EventStatus { get; set; }

        public virtual ICollection<Message> Messages { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
