using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SocialLife.Models
{
    [Table("Messages")]
    public partial class Message
    {
        //Database fields
        [Key]
        public int MessageId { get; set; }

        [Required]
        public string MessageContent { get; set; }

        [Required]
        public System.DateTime MessageDate { get; set; }

        //Database relationships

        public int? SenderId { get; set; }

        public int? ReceiverId { get; set; }

        public int? EventId { get; set; }

        //[InverseProperty("MessageSender")]
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        //[InverseProperty("MessageReceiver")]
        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        [ForeignKey("EventId")]
        public virtual Event EventMessages { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }

        public virtual Status Status { get; set; }
    }
}
