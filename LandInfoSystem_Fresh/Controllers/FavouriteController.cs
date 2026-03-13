using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LandInfoSystem.Data;

namespace LandInfoSystem.Controllers
{
    [Route("api/favourite")]
    [ApiController]
    public class FavouriteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavouriteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("{propertyId}")]
        public async Task<IActionResult> AddFavourite(int propertyId)
        {
            var property = await _context.LandProperties.FindAsync(propertyId);

            if (property == null)
                return NotFound();

            property.IsAvailable = false;

            await _context.SaveChangesAsync();

            return Ok(property);
        }
    }
}