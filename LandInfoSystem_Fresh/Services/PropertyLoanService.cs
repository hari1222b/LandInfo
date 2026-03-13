using LandInfoSystem.Data;
using LandInfoSystem.DTOs;
using LandInfoSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LandInfoSystem.Services
{
    public interface IPropertyLoanService
    {
        Task<LoanEligibilityResponseDto?> GetEligibilityAsync(int propertyId);
        Task<ApiResponse> ApplyPropertyLoanAsync(PropertyLoanApplicationDto request);
    }

    public class PropertyLoanService : IPropertyLoanService
    {
        private readonly ApplicationDbContext _context;

        public PropertyLoanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LoanEligibilityResponseDto?> GetEligibilityAsync(int propertyId)
        {
            var property = await _context.LandProperties
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId);

            if (property == null) return null;

            return new LoanEligibilityResponseDto
            {
                PropertyId = property.PropertyId,
                PropertyTitle = property.Title,
                PropertyPrice = property.Price,
                MaxLoanAmount = Math.Round(property.Price * 0.80m, 0),
                MinDownPayment = Math.Round(property.Price * 0.20m, 0),
                TenureOptions = new[] { 5, 10, 15 }
            };
        }

        public async Task<ApiResponse> ApplyPropertyLoanAsync(PropertyLoanApplicationDto request)
        {
            try
            {
                var loan = new LandLoanRequest
                {
                    UserId = request.UserId,
                    PropertyId = request.PropertyId,
                    LoanAmount = Math.Round(request.LoanAmount, 0),
                    DownPayment = Math.Round(request.DownPayment, 0),
                    InterestRate = request.InterestRate,
                    TenureYears = request.TenureYears,
                    MonthlyEMI = Math.Round(request.MonthlyEMI, 0),
                    TotalInterest = Math.Round(request.TotalInterest, 0),
                    TotalPayment = Math.Round(request.TotalPayment, 0),
                    Status = "Pending",
                    CreatedDate = DateTime.UtcNow
                };

                _context.LandLoanRequests.Add(loan);
                await _context.SaveChangesAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = "Loan Application Submitted Successfully",
                    Data = loan.Id
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = $"Failed to submit loan: {ex.Message}"
                };
            }
        }
    }
}
