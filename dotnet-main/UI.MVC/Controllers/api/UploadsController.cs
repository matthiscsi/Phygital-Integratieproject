using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;

namespace UI.MVC.Controllers.api;
[ApiController]
[Route("/api/[controller]")]
public class UploadsController : ControllerBase
{
    private readonly ICloudBucketManager _cloudBucketManager;
    
    public UploadsController(ICloudBucketManager cloudBucketManager)
    {
        _cloudBucketManager = cloudBucketManager;
    }
    
    [HttpPost("upload")]
    [Authorize]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        var url = await _cloudBucketManager.UploadOnlyFile(file);
        return Ok(new { url });
    }
}