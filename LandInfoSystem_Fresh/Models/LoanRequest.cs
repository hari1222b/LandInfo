using System;
using System.ComponentModel.DataAnnotations;

namespace LandInfoSystem.Models
{
    public class LoanRequest
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        
        [Required]
        public decimal PrincipalAmount { get; set; }
        
        [Required]
        public decimal InterestRate { get; set; }
        
        [Required]
        public int TenureYears { get; set; }
        
        [Required]
        public decimal DownPayment { get; set; }
        
        public decimal MonthlyEMI { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalPayment { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
