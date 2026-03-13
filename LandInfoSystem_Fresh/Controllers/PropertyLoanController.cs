using LandInfoSystem.DTOs;
using LandInfoSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LandInfoSystem.Controllers
{
    [ApiController]
    [Route("api/propertyloan")]
    public class PropertyLoanController : ControllerBase
    {
        private readonly IPropertyLoanService _loanService;

        public PropertyLoanController(IPropertyLoanService loanService)
        {
            _loanService = loanService;
        }

        // GET /api/propertyloan/eligibility/{propertyId}
        [HttpGet("eligibility/{propertyId}")]
        public async Task<IActionResult> GetEligibility(int propertyId)
        {
            if (propertyId <= 0)
                return BadRequest(new ApiResponse { Success = false, Message = "Invalid property ID." });

            var result = await _loanService.GetEligibilityAsync(propertyId);

            if (result == null)
                return NotFound(new ApiResponse { Success = false, Message = "Property not found." });

            return Ok(result);
        }

        // POST /api/propertyloan/apply
        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] PropertyLoanApplicationDto request)
        {
            if (request == null || request.PropertyId <= 0 || request.LoanAmount <= 0)
                return BadRequest(new ApiResponse { Success = false, Message = "Invalid loan request." });

            var result = await _loanService.ApplyPropertyLoanAsync(request);

            if (result.Success) return Ok(result);
            return StatusCode(500, result);
        }
    }
}
