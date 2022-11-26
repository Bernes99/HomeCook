using HomeCook.Data.Models;
using HomeCook.Data.Models.CustomModels;
using HomeCook.DTO;
using HomeCook.DTO.Pagination;
using HomeCook.DTO.ResetPassword;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace HomeCook.Services.Interfaces
{
    public interface IAuthService
    {
        AuthenticationProperties ConfigureExternalAuthProp(string provider, string redirectUrl);
        Task<IdentityResult> ConfirmEmailAddress(string userId, string code);
        Task<AuthenticationResponse> CreateJwtTokens(AppUser user);
        Task<IdentityResult> DeleteUser(string userId);
        Task<AuthenticationResponse> ExternalLogin(ExternalAuthDto externalAuth);
        Task<AppUser> FindUserAsync(string emailAddress);
        Task<IdentityResult> ForgotPassword(string email);
        string GenerateRefreshToken();
        Task<ExternalLoginInfo> GetExternalLoginInfo();
        Task<IList<Claim>> GetUserClaimsAsync(AppUser user);
        PaginationResult<UserDto> GetUsers(PaginationQuery searchPhrase);
        Task<AuthenticationResponse?> Login(LoginDto model);
        Task Logout();
        Task<AuthenticationResponse> Refresh(string refreshToken);
        Task<AppUser> Register(RegisterDto registerDto);
        Task<IdentityResult> ResetPassword(ResetPasswordDto resetPassword);
        void test();
        Task<IdentityResult> UpdateUser(string userId, UserUpdateDto model);
    }
}