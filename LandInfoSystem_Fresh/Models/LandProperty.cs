namespace LandInfoSystem.Models
{
    public class LandProperty
    {
        public int PropertyId { get; set; }
        public int SellerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Area { get; set; } // in square feet
        public decimal Price { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? BoundaryJson { get; set; } // GeoJSON format
        public DateTime? UpdatedDate { get; set; }

        // Navigation Properties
        public User Seller { get; set; }
        public ICollection<Request> Requests { get; set; } = new List<Request>();
        public ICollection<LandDocument> Documents { get; set; } = new List<LandDocument>();
    }
}
