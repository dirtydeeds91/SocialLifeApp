using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialLife.Services.Models
{
    public class MessageModel
    {
        public int MessageId { get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }

        public string Sender { get; set; }

        public string Receiver { get; set; }

        public int ReceiverId { get; set; }

        public string Event { get; set; }

        public int EventId { get; set; }

        public string Status { get; set; }
    }
}