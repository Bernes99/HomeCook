using Microsoft.AspNetCore.Mvc;
using HomeCook.DTO;
using HomeCook.Services.Interfaces;
using HomeCook.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Xml;

namespace HomeCook.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService AuthService;
        private const string JWT_TOKEN_COOKIE_NAME = "X-Access-Token";
        private const string REFRESH_TOKEN_COOKIE_NAME = "X-Refresh-Token";
        private AuthenticationSettings AuthSettings { get; }

        public AuthController(IAuthService authService, AuthenticationSettings authSettings)
        {
            AuthService = authService;
            AuthSettings = authSettings;
        }

        [HttpPost("refresh")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Refresh()
        {
            var refreshToken = Request.Cookies[REFRESH_TOKEN_COOKIE_NAME];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized();
            }

            var tokens = await AuthService.Refresh(refreshToken);

            if (tokens is null || String.IsNullOrEmpty(tokens.JwtToken) || String.IsNullOrEmpty(tokens.RefreshToken))
            {
                return BadRequest();
            }
            Response.Cookies.Append(JWT_TOKEN_COOKIE_NAME, tokens.JwtToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddHours(AuthSettings.JwtExpireHours) });
            //Response.Cookies.Append("X-Username", user.UserName, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            Response.Cookies.Append(REFRESH_TOKEN_COOKIE_NAME, tokens.RefreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddHours(AuthSettings.RefreshTokenExpireHours) });

            return Ok();
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            await AuthService.Register(registerDto);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login([FromBody] LoginDto model)
        {
            var tokens = await AuthService.Login(model);

            if (tokens is null || String.IsNullOrEmpty(tokens.JwtToken) || String.IsNullOrEmpty(tokens.RefreshToken))
            {
                return BadRequest();
            }

            Response.Cookies.Append(JWT_TOKEN_COOKIE_NAME, tokens.JwtToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddHours(AuthSettings.JwtExpireHours) });
            //Response.Cookies.Append("X-Username", user.UserName, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            Response.Cookies.Append(REFRESH_TOKEN_COOKIE_NAME, tokens.RefreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddHours(AuthSettings.RefreshTokenExpireHours) });

            return Ok(tokens);
        }

        [HttpGet("logout")]
        public async Task<ActionResult> Logout()
        {
            await AuthService.Logout();
            Response.Cookies.Append("X-Access-Token", "", new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddHours(-AuthSettings.JwtExpireHours) });
            //Response.Cookies.Append("X-Username", user.UserName, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            Response.Cookies.Append("X-Refresh-Token", "", new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddHours(-AuthSettings.RefreshTokenExpireHours) });

            return Ok();
        }

        [HttpGet("loggedIn")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<LoggedInResponse>> IsLogin()
        {
            var claims = User.Claims;
            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var publicIdClaim = claims.FirstOrDefault(c => c.Type == "PublicId");

            var result = new LoggedInResponse
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Email = emailClaim != null ? emailClaim.Value : null,
                Role = roleClaim != null ? roleClaim.Value : null,
                Id = publicIdClaim != null ? publicIdClaim.Value : null
            };
            return Ok(result);
        }

        [HttpPost("googleLoginv1")]
        public async Task<IActionResult> LoginGoogle(string provider, string returnUrl)
        {
            var redirectUrl = $"https://localhost:8081/api/auth/googleLoginCallback?returnUrl={returnUrl}";
            var properties = AuthService.ConfigureExternalAuthProp(provider, redirectUrl);
            properties.AllowRefresh = true;
            
            var result = Challenge(properties, provider);
            return Ok(result);
        }

        [HttpPost("googleLogin")]
        public async Task<IActionResult> LoginCallback([FromBody]ExternalAuthDto externalAuth)
        {
            var result = await AuthService.ExternalLogin(externalAuth);
            if (result is null)
            {
                return Unauthorized();
            }

            Response.Cookies.Append("X-Access-Token", result.JwtToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddHours(AuthSettings.JwtExpireHours) });
            //Response.Cookies.Append("X-Username", user.UserName, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict });
            Response.Cookies.Append("X-Refresh-Token", result.RefreshToken, new CookieOptions() { HttpOnly = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddHours(AuthSettings.RefreshTokenExpireHours) });
            return Ok(result);

        }

        [HttpGet("test")]
        public async Task<ActionResult<LoggedInResponse>> test()
        {
            AuthService.test();
            return Ok();
        }
    }
}
