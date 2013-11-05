using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SocialLife.Models
{
    [Table("Statuses")]
    public partial class Status
    {
        //Constructor
        public Status()
        {
            this.Profiles = new HashSet<Profile>();
            this.Events = new HashSet<Event>();
        }

        //Database fields
        [Key]
        public int StatusId { get; set; }

        [Required]
        public string StatusName { get; set; }

        //Database relationships
        public virtual ICollection<Profile> Profiles { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
