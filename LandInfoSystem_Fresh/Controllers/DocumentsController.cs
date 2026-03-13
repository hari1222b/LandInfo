using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LandInfoSystem.Data;
using LandInfoSystem.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LandInfoSystem.Controllers
{
    [Route("api/documents")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _uploadPath;

        public DocumentsController(ApplicationDbContext context)
        {
            _context = context;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/documents");
            
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file, [FromForm] int propertyId, [FromForm] int sellerId, [FromForm] string documentType)
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded");
            if (file.Length > 5 * 1024 * 1024) return BadRequest("File size exceeds 5MB limit");

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension)) return BadRequest("Invalid file type. Only PDF and Images are allowed.");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new LandDocument
            {
                PropertyId = propertyId,
                SellerId = sellerId,
                DocumentType = documentType,
                FileName = file.FileName,
                FilePath = "/uploads/documents/" + fileName,
                Status = "Pending",
                UploadedDate = DateTime.UtcNow
            };

            _context.LandDocuments.Add(document);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, document });
        }

        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetPropertyDocuments(int propertyId)
        {
            var docs = await _context.LandDocuments
                .Where(d => d.PropertyId == propertyId)
                .ToListAsync();
            return Ok(docs);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingDocuments()
        {
            var docs = await _context.LandDocuments
                .Where(d => d.Status == "Pending")
                .Include(d => d.Property)
                .ToListAsync();
            return Ok(docs);
        }

        [HttpPut("verify/{id}")]
        public async Task<IActionResult> VerifyDocument(int id, [FromBody] VerificationRequest request)
        {
            var doc = await _context.LandDocuments.FindAsync(id);
            if (doc == null) return NotFound();

            doc.Status = request.Status; // Verified or Rejected
            doc.RejectionReason = request.RejectionReason;
            
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        public class VerificationRequest
        {
            public string Status { get; set; }
            public string RejectionReason { get; set; }
        }
    }
}
