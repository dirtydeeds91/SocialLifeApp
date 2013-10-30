using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SocialLife.Models
{
    public partial class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required]
        public string Latitude { get; set; }

        [Required]
        public string Longitude { get; set; }

        [Required]
        public System.DateTime Date { get; set; }
    }
}
