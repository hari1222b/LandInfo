using Microsoft.AspNetCore.Mvc;
using LandInfoSystem.DTOs;
using LandInfoSystem.Services;

namespace LandInfoSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LandPropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        private readonly IFraudDetectionService _fraudService;

        public LandPropertiesController(IPropertyService propertyService, IFraudDetectionService fraudService)
        {
            _propertyService = propertyService;
            _fraudService = fraudService;
        }

        /// <summary>
        /// Get all properties
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PropertyResponseDto>>> GetAllProperties()
        {
            try
            {
                var properties = await _propertyService.GetAllPropertiesAsync();
                return Ok(properties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get only available properties
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<List<PropertyResponseDto>>> GetAvailableProperties()
        {
            try
            {
                var properties = await _propertyService.GetAvailablePropertiesAsync();
                return Ok(properties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get property by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyResponseDto>> GetPropertyById(int id)
        {
            try
            {
                var property = await _propertyService.GetPropertyByIdAsync(id);

                if (property == null)
                    return NotFound(new { message = "Property not found" });

                return Ok(property);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get properties by seller ID
        /// </summary>
        [HttpGet("seller/{sellerId}")]
        public async Task<ActionResult<List<PropertyResponseDto>>> GetPropertiesBySeller(int sellerId)
        {
            try
            {
                var properties = await _propertyService.GetPropertiesBySeller(sellerId);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Create new property
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PropertyResponseDto>> CreateProperty([FromBody] CreatePropertyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var property = await _propertyService.CreatePropertyAsync(dto);

                // Auto-run fraud detection in background (fire-and-forget)
                _ = Task.Run(async () =>
                {
                    try { await _fraudService.AnalyzePropertyAsync(property.PropertyId); }
                    catch { /* non-blocking */ }
                });

                return CreatedAtAction(nameof(GetPropertyById), new { id = property.PropertyId }, property);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Update property
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PropertyResponseDto>> UpdateProperty(int id, [FromBody] UpdatePropertyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var property = await _propertyService.UpdatePropertyAsync(id, dto);

                if (property == null)
                    return NotFound(new { message = "Property not found" });

                return Ok(property);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Delete property
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProperty(int id)
        {
            try
            {
                var success = await _propertyService.DeletePropertyAsync(id);

                if (!success)
                    return NotFound(new { message = "Property not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        /// <summary>
        /// Search properties by city and price
        /// </summary>
        [HttpGet("search")]
        /// <summary>
        /// Update property boundary
        /// </summary>
        [HttpPost("{id}/boundary")]
        public async Task<ActionResult<PropertyResponseDto>> UpdateBoundary(int id, [FromBody] string boundaryJson)
        {
            try
            {
                var property = await _propertyService.UpdatePropertyBoundaryAsync(id, boundaryJson);

                if (property == null)
                    return NotFound(new { message = "Property not found" });

                return Ok(property);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }

        public async Task<ActionResult<List<PropertyResponseDto>>> SearchProperties(
            [FromQuery] string city,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice)
        {
            try
            {
                var properties = await _propertyService.SearchPropertiesAsync(city, minPrice, maxPrice);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error: {ex.Message}" });
            }
        }
    }
}
