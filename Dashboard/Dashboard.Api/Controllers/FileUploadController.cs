
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public FileUploadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("images")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            try
            {
                // Use wwwroot/uploads instead of just uploads
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // The rest of your code...
                var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var url = $"{Request.Scheme}://{Request.Host}/uploads/images/{uniqueFileName}";
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("documents")]
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            // Validate file type
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type. Only pdf, doc, docx, xls, and xlsx are allowed.");
            }

            try
            {
                // Create upload directory if it doesn't exist
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "documents");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename but keep original name for reference
                var uniqueFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{Guid.NewGuid().ToString().Substring(0, 8)}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Return URL to access the file
                var url = $"{Request.Scheme}://{Request.Host}/uploads/documents/{uniqueFileName}";

                return Ok(new { url, fileName = file.FileName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
