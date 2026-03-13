using Microsoft.AspNetCore.Mvc;
using LandInfoSystem.Data;
using LandInfoSystem.Models;
using LandInfoSystem.DTOs;

namespace LandInfoSystem.Controllers
{
    [Route("api/contact")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SendRequest([FromBody] CreateRequestDto dto)
        {
            if (dto == null) return BadRequest("Invalid request data");

            var request = new Request
            {
                BuyerId = dto.BuyerId,
                PropertyId = dto.PropertyId,
                Message = dto.Message,
                Status = "Pending",
                CreatedDate = DateTime.UtcNow
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Inquiry sent successfully" });
        }
    }
}