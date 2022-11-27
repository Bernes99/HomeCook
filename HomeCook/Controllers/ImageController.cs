using HomeCook.Data.CustomException;
using HomeCook.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeCook.Controllers
{
    [Route("api/Image")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ImageController : Controller
    {
        private readonly IImageService ImageService;

        public ImageController(IImageService imageService)
        {
            ImageService = imageService;
        }

        [HttpPost("ProfileImage/{userId}")]
        public async Task<ActionResult> UpdateProfileImage([FromRoute] string userId, IFormFile? file)
        {
            ImageService.UpdateProfileImage(file, userId);
            return Ok();
        }

        [HttpGet("ProfileImage/{userId}")]
        public async Task<ActionResult> GetProfileImage([FromRoute] string userId)
        {
            var result = ImageService.GetProfileImage(userId);

            return Ok(result);

            //Byte[] b = System.IO.File.ReadAllBytes(@"C:\Users\lukas\Desktop\test.png");
            //return File(b, "image/jpeg");
        }
    }
}
