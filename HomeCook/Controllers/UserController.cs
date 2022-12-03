using HomeCook.Data.Models.CustomModels;
using HomeCook.DTO;
using HomeCook.Services;
using HomeCook.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCook.Controllers
{
    [Route("api/User")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IImageService _imageService;

        public UserController(IUserService userService, IImageService imageService)
        {
            _userService = userService;
            _imageService = imageService;
        }


        [HttpGet("GetAllUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> GetAllUsers([FromQuery] PaginationQuery query)
        {
            var users = _userService.GetUsers(query);
            return Ok(users);
        }
        [HttpDelete("{Id}/DeleteUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin,User")]
        public async Task<ActionResult> DeleteUser([FromRoute] string Id)
        {
            if (!IsSelfOrAdmin(Id))
            {
                return BadRequest();
            }

            var result = await _userService.DeleteUser(Id);
            if (result.Succeeded)
            {
                return Ok();
            }
            return StatusCode(500, result.Errors);
        }
        [HttpPost("{Id}/UpdateUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin,User")]
        public async Task<ActionResult> UpdateUser([FromRoute] string Id, [FromForm] UserUpdateDto model)
        {
            if (!IsSelfOrAdmin(Id))
            {
                return BadRequest();
            }
            
            var result = await _userService.UpdateUser(Id, model);
            if (result.Succeeded)
            {
                return Ok();
            }
            return StatusCode(500, result.Errors);
        }

        [HttpPost("{Id}/ProfileImage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin,User")]
        public async Task<ActionResult> UpdateProfileImage([FromRoute] string Id, IFormFile? file)
        {
            if (!IsSelfOrAdmin(Id))
            {
                return BadRequest();
            }
            _imageService.UpdateProfileImage(file, Id);
            return Ok();
        }

        [HttpGet("{Id}/ProfileImage")]
        public async Task<ActionResult> GetProfileImage([FromRoute] string Id)
        {
            var result = _imageService.GetProfileImage(Id);

            return Ok(result);

            //Byte[] b = System.IO.File.ReadAllBytes(@"C:\Users\lukas\Desktop\test.png");
            //return File(b, "image/jpeg");
        }
        private bool IsSelfOrAdmin(string uesrId)
        {
            if (!User.IsInRole("Admin"))
            {
                var requestUserId = User.Claims.FirstOrDefault(x => x.Type == "PublicId")?.Value;
                if (requestUserId != uesrId)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
