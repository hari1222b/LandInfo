using Microsoft.AspNetCore.Mvc;
using LandInfoSystem.DTOs;
using LandInfoSystem.Services;
using System.Threading.Tasks;

namespace LandInfoSystem.Controllers
{
    [ApiController]
    [Route("api/land")]
    public class LandPredictionController : ControllerBase
    {
        private readonly ILandPredictionService _predictionService;

        public LandPredictionController(ILandPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpPost("predict")]
        public async Task<ActionResult<LandPredictionResponseDto>> Predict([FromBody] LandPredictionRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Basic validation
            if (dto.LandSize <= 0)
                return BadRequest("Land size must be greater than zero.");
            if (dto.DistanceSchool < 0 || dto.DistanceHospital < 0)
                return BadRequest("Distances cannot be negative.");

            var result = await _predictionService.PredictAsync(dto);
            return Ok(result);
        }
    }
}
