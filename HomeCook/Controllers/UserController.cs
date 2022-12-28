using HomeCook.Data.Models.CustomModels;
using HomeCook.DTO;
using HomeCook.DTO.Product;
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
        private readonly IProductService _productService;
        private readonly IRecipeService _recipeService;

        public UserController(IUserService userService, IImageService imageService, IProductService productService, IRecipeService recipeService)
        {
            _userService = userService;
            _imageService = imageService;
            _productService = productService;
            _recipeService = recipeService;
        }

        #region User
        [HttpGet("GetAllUsers")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin")]
        public async Task<ActionResult> GetAllUsers([FromQuery] UserPaginationQuery query)
        {
            var users = _userService.GetUsers(query);
            return Ok(users);
        }

        [HttpGet("{Id}/Details")]
        public async Task<ActionResult> GetUser([FromRoute] string Id)
        {
            if (!IsSelfOrAdmin(Id))
            {
                return Unauthorized();
            }
            var user = await _userService.GetUserDto(Id);
            return Ok(user);
        }

        [HttpDelete("{Id}/DeleteUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin,User")]
        public async Task<ActionResult> DeleteUser([FromRoute] string Id)
        {
            if (!IsSelfOrAdmin(Id))
            {
                return Unauthorized();
            }

            var result = await _userService.DeleteUser(Id);
            if (result.Succeeded)
            {
                return Ok();
            }
            return StatusCode(500, result.Errors);
        }
        [HttpPut("{Id}/UpdateUser")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin,User")]
        public async Task<ActionResult> UpdateUser([FromRoute] string Id, [FromBody] UserUpdateDto model)
        {
            if (!IsSelfOrAdmin(Id))
            {
                return Unauthorized();
            }
            
            var result = await _userService.UpdateUser(Id, model);
            if (result.Succeeded)
            {
                return Ok();
            }
            return StatusCode(500, result.Errors);
        }
        #endregion
        #region profileImage
        [HttpPut("{Id}/ProfileImage")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin,User")]
        public async Task<ActionResult> UpdateProfileImage([FromRoute] string Id, IFormFile? file)
        {
            if (!IsSelfOrAdmin(Id))
            {
                return Unauthorized();
            }
            _imageService.UpdateProfileImage(file, Id);
            return Ok();
        }
        [AllowAnonymous]
        [HttpGet("{Id}/ProfileImage")]
        public async Task<ActionResult> GetProfileImage([FromRoute] string Id)
        {
            var result = _imageService.GetProfileImage(Id);

            return Ok(result);
        }
        #endregion


        [HttpPost("{Id}/Products/AddOrUpdate")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin,User")]
        public async Task<ActionResult> UpdateUserProduct([FromRoute] string Id, [FromBody] List<AddUserProductDto> model)
        {
            if (model is null || !model.Any())
            {
                return BadRequest();
            }
            if (!IsSelfOrAdmin(Id))
            {
                return Unauthorized();
            }
            await _productService.AddOrUpdateUserProducts(model, Id);

            return Ok();
        }

        [HttpDelete("{Id}/Products/Remove")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin,User")]
        public async Task<ActionResult> RemoveUserProduct([FromRoute] string Id, IdsDto model)
        {
            if (model is null || !model.Id.Any())
            {
                return BadRequest();
            }
            if (!IsSelfOrAdmin(Id))
            {
                return Unauthorized();
            }
            await _productService.DeleteUserProduct(model, Id);

            return Ok();
        }

        [HttpGet("{Id}/Products")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin,User")]
        public async Task<ActionResult> GetUserProdusctsList([FromRoute] string Id)
        {
            if (!IsSelfOrAdmin(Id))
            {
                return Unauthorized();
            }
            var result = await _productService.GetUserProductList(Id);
            return Ok(result);
        }

        [HttpGet("{Id}/Recipes")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Roles = "Admin,User")]
        public async Task<ActionResult> GetRecipe([FromRoute] string Id)
        {
            if (!IsSelfOrAdmin(Id))
            {
                return Unauthorized();
            }
            var result = await _recipeService.GetUserRecipes(Id);

            return Ok(result);
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
