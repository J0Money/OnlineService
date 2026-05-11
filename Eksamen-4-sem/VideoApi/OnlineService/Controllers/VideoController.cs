using Microsoft.AspNetCore.Mvc;
using OnlineService.Models;
using OnlineService.Repositories.Interfaces;

namespace OnlineService.Controllers;

[ApiController]
[Route("api/videos")]
public class VideoController : ControllerBase
{
    private readonly IVideoRepository _videoRepository;
    private readonly ILogger<VideoController> _logger;

    public VideoController(
        IVideoRepository videoRepository,
        ILogger<VideoController> logger)
    {
        _videoRepository = videoRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVideos()
    {
        _logger.LogInformation("Fetching all videos");
        
        var videos = await _videoRepository.GetAllVideos();

        return Ok(videos);
    }

    [HttpGet("{videoId}")]
    public async Task<IActionResult> GetVideo(string videoId)
    {
        var video = await _videoRepository.GetVideoById(videoId);

        if (video == null)
            return NotFound();

        return Ok(video);
    }
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadVideo([FromForm] VideoDto dto)
    {
        
        _logger.LogInformation("Upload request received");
        
        if (dto.VideoFile == null || dto.VideoFile.Length == 0)
        {
            _logger.LogWarning("Upload failed: no video file was provided");
            return BadRequest("Video file is required");
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
            _logger.LogInformation("Uploads folder created");
        }

        var fileName = Guid.NewGuid() + Path.GetExtension(dto.VideoFile.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.VideoFile.CopyToAsync(stream);
        }
        
        _logger.LogInformation("Video file uploaded: {FileName}", fileName);


        var video = new Video
        {
            Title = dto.Title,
            Description = dto.Description,
            VideoUrl = fileName,
            UploadDate = DateTime.UtcNow
        };

        await _videoRepository.UploadVideo(video);
        
        _logger.LogInformation("Video metadata saved to MongoDB: {Title}", dto.Title);

        return Created();
    }

    [HttpDelete("{videoId}")]
    public async Task<IActionResult> DeleteVideo(string videoId)
    {
        _logger.LogInformation("Delete request received for video: {VideoId}", videoId);
        
        var deleted = await _videoRepository.DeleteVideo(videoId);

        if (!deleted)
        {
            _logger.LogWarning("Delete failed. Video not found: {VideoId}", videoId);
            return NotFound();
        }
        _logger.LogInformation("Video deleted successfully: {VideoId}", videoId);
        return NoContent();
    }
}