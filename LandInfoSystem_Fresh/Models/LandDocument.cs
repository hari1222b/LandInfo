using System;
using System.ComponentModel.DataAnnotations;

namespace LandInfoSystem.Models
{
    public class LandDocument
    {
        [Key]
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int SellerId { get; set; }
        
        [Required]
        public string DocumentType { get; set; } // Sale Deed, Patta, Ownership Proof, EC
        
        [Required]
        public string FilePath { get; set; }
        
        [Required]
        public string FileName { get; set; }
        
        public string Status { get; set; } = "Pending"; // Pending, Verified, Rejected
        public string RejectionReason { get; set; }
        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public LandProperty Property { get; set; }
    }
}
