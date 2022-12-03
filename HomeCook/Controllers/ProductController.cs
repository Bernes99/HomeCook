using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeCook.Controllers
{
    [Route("api/Product")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductController : Controller
    {
        [HttpPost("AddProductCategory")]
        public async Task<ActionResult> AddProductCategory(string CategoryName)
        {

            //var result = await _userService.DeleteUser(Id);
            //if (result.Succeeded)
            //{
                return Ok();
            //}
            //return StatusCode(500, result.Errors);
        }
    }
}
