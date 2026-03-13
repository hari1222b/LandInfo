using Microsoft.AspNetCore.Mvc;
using System.Linq;
using LandInfoSystem.Data;
using LandInfoSystem.Models;

namespace LandInfoSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult SendMessage(Message message)
        {
            _context.Messages.Add(message);
            _context.SaveChanges();

            return Ok(message);
        }

        [HttpGet("seller/{sellerId}")]
        public IActionResult GetSellerMessages(int sellerId)
        {
            var messages = _context.Messages
                .Where(m => m.SellerId == sellerId)
                .OrderByDescending(m => m.CreatedDate)
                .ToList();

            return Ok(messages);
        }
    }
}