namespace LandInfoSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? UserType { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<LandProperty> LandProperties { get; set; } = new List<LandProperty>();
        public ICollection<Request> BuyerRequests { get; set; } = new List<Request>();
    }
}