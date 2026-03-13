using System;

namespace LandInfoSystem.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public int PropertyId { get; set; }

        public int SellerId { get; set; }

        public string BuyerName { get; set; }

        public string BuyerPhone { get; set; }
        public string BuyerEmail { get; set; }
        public string MessageText { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}