namespace LandInfoSystem.Models
{
    public class Request
    {
        public int RequestId { get; set; }
        public int BuyerId { get; set; }
        public int PropertyId { get; set; }
        public string Message { get; set; }
        public string Status { get; set; } // Pending, Accepted, Rejected
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public User Buyer { get; set; }
        public LandProperty Property { get; set; }
    }
}
