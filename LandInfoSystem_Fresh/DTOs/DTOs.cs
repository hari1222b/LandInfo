using System.ComponentModel.DataAnnotations;

namespace LandInfoSystem.DTOs
{
    public class UserRegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string UserType { get; set; } = "Buyer"; // Buyer, Seller, Agent, Admin
    }

    public class UserLoginDto
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public class UserResponseDto
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserType { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }
        public int? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; } // Needed for SMS mapping
        public string? UserType { get; set; }
    }

    public class RegisterResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class CreatePropertyDto
    {
        public int SellerId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Area { get; set; }
        public decimal Price { get; set; }
        public string? Location { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public string? ImageUrl { get; set; }
    }
    public class UpdatePropertyDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Area { get; set; }
        public decimal Price { get; set; }
        public string? Location { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public string? ImageUrl { get; set; }
        public string? BoundaryJson { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class PropertyResponseDto
    {
        public int PropertyId { get; set; }
        public int SellerId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal Area { get; set; }
        public decimal Price { get; set; }
        public string? Location { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PinCode { get; set; }
        public string? ImageUrl { get; set; }
        public string? BoundaryJson { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public UserResponseDto? Seller { get; set; }
    }

    public class CreateRequestDto
    {
        public int BuyerId { get; set; }
        public int PropertyId { get; set; }
        public string? Message { get; set; }
    }

    public class RequestResponseDto
    {
        public int RequestId { get; set; }
        public int BuyerId { get; set; }
        public int PropertyId { get; set; }
        public string? Message { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public UserResponseDto? Buyer { get; set; }
        public PropertyResponseDto? Property { get; set; }
    }

    public class ChatResponse
    {
        public string Response { get; set; }
    }

    public class LandPredictionRequestDto
    {
        public string? Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        
        public double LandSize { get; set; }
        public bool RoadAccess { get; set; }
        public bool WaterFacility { get; set; }
        public double DistanceSchool { get; set; }
        public double DistanceHospital { get; set; }
    }

    public class PriceTrendPoint
    {
        public string Month { get; set; }
        public decimal Price { get; set; }
    }

    public class LandPredictionResponseDto
    {
        public decimal PredictedPrice { get; set; }
        public List<PriceTrendPoint> PriceTrend { get; set; }
    }

    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }

    // --- LOAN OFFER DTOs ---
    public class ApplyLoanRequestDto
    {
        public int UserId { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int TenureYears { get; set; }
        public decimal DownPayment { get; set; }
        public decimal MonthlyEMI { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalPayment { get; set; }
    }

    // --- PROPERTY LOAN DTOs ---
    public class LoanEligibilityResponseDto
    {
        public int PropertyId { get; set; }
        public string? PropertyTitle { get; set; }
        public decimal PropertyPrice { get; set; }
        public decimal MaxLoanAmount { get; set; }      // 80% of price
        public decimal MinDownPayment { get; set; }     // 20% of price
        public int[] TenureOptions { get; set; } = new[] { 5, 10, 15 };
    }

    public class PropertyLoanApplicationDto
    {
        public int UserId { get; set; }
        public int PropertyId { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal DownPayment { get; set; }
        public decimal InterestRate { get; set; }
        public int TenureYears { get; set; }
        public decimal MonthlyEMI { get; set; }
        public decimal TotalInterest { get; set; }
        public decimal TotalPayment { get; set; }
    }

    // --- FRAUD DETECTION DTOs ---
    public class FraudAnalysisResponseDto
    {
        public int PropertyId { get; set; }
        public int FraudScore { get; set; }
        public string RiskLevel { get; set; } = "Low";
        public bool IsDuplicate { get; set; }
        public bool IsPriceAnomaly { get; set; }
        public bool IsSuspiciousSeller { get; set; }
        public string? WarningMessage { get; set; }
        public DateTime AnalyzedAt { get; set; }
    }
}