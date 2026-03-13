using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LandInfoSystem.Data;
using LandInfoSystem.Models;

namespace LandInfoSystem.Controllers
{
    [Route("api/propertysearch")]
    [ApiController]
    public class PropertySearchController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PropertySearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string? city, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.LandProperties.AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(p => p.City.Contains(city));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var result = await query.ToListAsync();

            return Ok(result);
        }
    }
}