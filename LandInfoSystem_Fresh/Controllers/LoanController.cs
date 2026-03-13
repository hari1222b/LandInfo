using LandInfoSystem.DTOs;
using LandInfoSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LandInfoSystem.Controllers
{
    [ApiController]
    [Route("api/loan")]
    public class LoanController : ControllerBase
    {
        private readonly ILoanService _loanService;

        public LoanController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyLoanRequestDto request)
        {
            if (request == null || request.UserId <= 0 || request.PrincipalAmount <= 0)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid loan request parameters."
                });
            }

            var result = await _loanService.ApplyForLoanAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return StatusCode(500, result);
        }
    }
}
