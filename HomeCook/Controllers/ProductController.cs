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
        //[HttpPost("Add")]
        //public async Task<ActionResult> AddProduct([FromBody] )
        //{

        //    var result = await _userService.DeleteUser(Id);
        //    if (result.Succeeded)
        //    {
        //        return Ok();
        //    }
        //    return StatusCode(500, result.Errors);
        //}
    }
}
