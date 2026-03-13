using System;
using System.ComponentModel.DataAnnotations;

namespace LandInfoSystem.Models
{
    public class LandLoanRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public decimal LoanAmount { get; set; }

        [Required]
        public int TenureYears { get; set; }

        public decimal MonthlyEMI { get; set; }
        public decimal DownPayment { get; set; }
        public decimal InterestRate { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalPayment { get; set; }

        // Pending / Approved / Rejected
        public string Status { get; set; } = "Pending";

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
