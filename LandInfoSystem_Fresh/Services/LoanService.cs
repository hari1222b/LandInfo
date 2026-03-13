using LandInfoSystem.Data;
using LandInfoSystem.DTOs;
using LandInfoSystem.Models;
using System.Threading.Tasks;
using System;

namespace LandInfoSystem.Services
{
    public interface ILoanService
    {
        Task<ApiResponse> ApplyForLoanAsync(ApplyLoanRequestDto request);
    }

    public class LoanService : ILoanService
    {
        private readonly ApplicationDbContext _context;

        public LoanService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse> ApplyForLoanAsync(ApplyLoanRequestDto request)
        {
            try
            {
                var newLoan = new LoanRequest
                {
                    UserId = request.UserId,
                    PrincipalAmount = request.PrincipalAmount,
                    InterestRate = request.InterestRate,
                    TenureYears = request.TenureYears,
                    DownPayment = request.DownPayment,
                    MonthlyEMI = request.MonthlyEMI,
                    TotalInterest = request.TotalInterest,
                    TotalPayment = request.TotalPayment,
                    CreatedAt = DateTime.UtcNow
                };

                _context.LoanRequests.Add(newLoan);
                await _context.SaveChangesAsync();

                return new ApiResponse
                {
                    Success = true,
                    Message = "Loan Application Submitted Successfully",
                    Data = newLoan.Id
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = $"Failed to submit loan request: {ex.Message}"
                };
            }
        }
    }
}
