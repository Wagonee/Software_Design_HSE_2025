using FileStoringService.Data;
using FileStoringService.Models;
using FileStoringService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using FileStoringService.DTOs;

namespace FileStoringService.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileStoringDbContext _context;
        private readonly IFileStorageProvider _fileStorageProvider;
        private readonly ILogger<FilesController> _logger;

        public FilesController(FileStoringDbContext context, IFileStorageProvider fileStorageProvider, ILogger<FilesController> logger)
        {
            _context = context;
            _fileStorageProvider = fileStorageProvider;
            _logger = logger;
        }

        private async Task<string> CalculateFileHashAsync(Stream stream)
        {
            using (var sha256 = SHA256.Create())
            {
                stream.Position = 0;
                var hashBytes = await sha256.ComputeHashAsync(stream);
                stream.Position = 0; 
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        [HttpPost("upload")]
        [ProducesResponseType(typeof(FileMetadataDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(FileMetadataDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("UploadFile called with no file or empty file.");
                return BadRequest("File is not provided or empty.");
            }

            if (Path.GetExtension(file.FileName).ToLowerInvariant() != ".txt")
            {
                _logger.LogWarning("UploadFile called with non-.txt file: {FileName}", file.FileName);
                return BadRequest("Only .txt files are allowed.");
            }

            _logger.LogInformation("UploadFile called for file: {FileName}, Size: {FileSize}", file.FileName, file.Length);

            try
            {
                string fileHash;
                await using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileHash = await CalculateFileHashAsync(memoryStream);

                    var existingFile = await _context.StoredFiles.AsNoTracking() 
                        .FirstOrDefaultAsync(f => f.Hash == fileHash);
                    if (existingFile != null)
                    {
                        _logger.LogInformation("File with hash {FileHash} already exists with ID: {FileId}. Original name: {OriginalFileName}", fileHash, existingFile.Id, existingFile.OriginalFileName);
                        return Ok(new FileMetadataDto
                        {
                            Id = existingFile.Id,
                            OriginalFileName = existingFile.OriginalFileName,
                            ContentType = existingFile.ContentType,
                            FileSize = existingFile.FileSize,
                            UploadedAt = existingFile.UploadedAt,
                            Hash = existingFile.Hash
                        });
                    }

                    var storedFileNameOnDisk = await _fileStorageProvider.SaveFileAsync(memoryStream, file.FileName, file.ContentType);

                    var newStoredFile = new StoredFile
                    {
                        Id = Guid.NewGuid(),
                        OriginalFileName = file.FileName,
                        ContentType = file.ContentType,
                        Hash = fileHash,
                        StoredFileName = storedFileNameOnDisk, 
                        FileSize = file.Length,
                        UploadedAt = DateTime.UtcNow
                    };

                    _context.StoredFiles.Add(newStoredFile);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("New file stored with ID: {FileId}, Hash: {FileHash}, Original name: {OriginalFileName}", newStoredFile.Id, newStoredFile.Hash, newStoredFile.OriginalFileName);

                    return CreatedAtAction(nameof(GetFileMetadata), new { id = newStoredFile.Id }, new FileMetadataDto
                    {
                        Id = newStoredFile.Id,
                        OriginalFileName = newStoredFile.OriginalFileName,
                        ContentType = newStoredFile.ContentType,
                        FileSize = newStoredFile.FileSize,
                        UploadedAt = newStoredFile.UploadedAt,
                        Hash = newStoredFile.Hash
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during file upload for {FileName}.", file.FileName);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred.");
            }
        }

        [HttpGet("{id}/metadata")]
        [ProducesResponseType(typeof(FileMetadataDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFileMetadata(Guid id)
        {
            _logger.LogInformation("GetFileMetadata called for ID: {FileId}", id);
            var storedFile = await _context.StoredFiles.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);

            if (storedFile == null)
            {
                _logger.LogWarning("File metadata not found for ID: {FileId}", id);
                return NotFound();
            }

            return Ok(new FileMetadataDto
            {
                Id = storedFile.Id,
                OriginalFileName = storedFile.OriginalFileName,
                ContentType = storedFile.ContentType,
                FileSize = storedFile.FileSize,
                UploadedAt = storedFile.UploadedAt,
                Hash = storedFile.Hash
            });
        }

        [HttpGet("{id}/download")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            _logger.LogInformation("DownloadFile called for ID: {FileId}", id);
            var storedFile = await _context.StoredFiles.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);

            if (storedFile == null)
            {
                _logger.LogWarning("File metadata not found for download, ID: {FileId}", id);
                return NotFound("File metadata not found.");
            }

            try
            {
                var fileStream = await _fileStorageProvider.GetFileAsync(storedFile.StoredFileName);
                _logger.LogInformation("Streaming file {OriginalFileName} (ID: {FileId}) for download.", storedFile.OriginalFileName, id);
                return File(fileStream, storedFile.ContentType, storedFile.OriginalFileName);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "Physical file not found for StoredFileName: {StoredFileName}, ID: {FileId}", storedFile.StoredFileName, id);
                return NotFound(ex.Message); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during file download for ID: {FileId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An internal server error occurred while retrieving the file.");
            }
        }
    }
}