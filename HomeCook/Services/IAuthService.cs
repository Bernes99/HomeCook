using HomeCook.Data.Models;
using HomeCook.Data.Models.CustomModels;
using HomeCook.DTO;
using HomeCook.DTO.Pagination;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HomeCook.Services
{
    public interface IAuthService
    {
        AuthenticationProperties ConfigureExternalAuthProp(string provider, string redirectUrl);
        Task<AuthenticationResponse> CreateJwtTokens(AppUser user);
        Task<AuthenticationResponse> ExternalLogin(ExternalAuthDto externalAuth);
        Task<AppUser> FindUserAsync(string emailAddress);
        string GenerateRefreshToken();
        Task<ExternalLoginInfo> GetExternalLoginInfo();
        Task<IList<Claim>> GetUserClaimsAsync(AppUser user);
        PaginationResult<UserDto> GetUsers(PaginationQuery searchPhrase);
        Task<AuthenticationResponse?> Login(LoginDto model);
        Task Logout();
        Task<AuthenticationResponse> Refresh(string refreshToken);
        Task<AppUser> Register(RegisterDto registerDto);
        void test();
    }
}