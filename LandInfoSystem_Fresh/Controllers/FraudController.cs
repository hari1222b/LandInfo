using Microsoft.AspNetCore.Mvc;
using LandInfoSystem.Services;
using System.Threading.Tasks;

namespace LandInfoSystem.Controllers
{
    [ApiController]
    [Route("api/fraud")]
    public class FraudController : ControllerBase
    {
        private readonly IFraudDetectionService _fraudService;

        public FraudController(IFraudDetectionService fraudService)
        {
            _fraudService = fraudService;
        }

        /// <summary>
        /// Get fraud analysis for a property (runs fresh if not exists)
        /// </summary>
        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetFraudAnalysis(int propertyId)
        {
            try
            {
                var result = await _fraudService.GetFraudAnalysisAsync(propertyId);
                if (result == null)
                    return NotFound(new { message = "Property not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Force re-analyze a property for fraud
        /// </summary>
        [HttpPost("analyze/{propertyId}")]
        public async Task<IActionResult> ReAnalyze(int propertyId)
        {
            try
            {
                var result = await _fraudService.AnalyzePropertyAsync(propertyId);
                if (result == null)
                    return NotFound(new { message = "Property not found" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }
    }
}
