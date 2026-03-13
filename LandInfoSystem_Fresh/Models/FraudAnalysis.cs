using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LandInfoSystem.Models
{
    public class FraudAnalysis
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PropertyId { get; set; }

        /// <summary>0–100 composite fraud score</summary>
        public int FraudScore { get; set; }

        /// <summary>Low / Medium / High</summary>
        public string RiskLevel { get; set; } = "Low";

        public bool IsDuplicate { get; set; }
        public bool IsPriceAnomaly { get; set; }
        public bool IsSuspiciousSeller { get; set; }

        public string? WarningMessage { get; set; }

        public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        [ForeignKey("PropertyId")]
        public LandProperty? Property { get; set; }
    }
}
