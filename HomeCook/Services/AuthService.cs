using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using HomeCook.DTO;
using HomeCook.Data;
using HomeCook.Data.Extensions;
using HomeCook.Data.Models;
using HomeCook.Services.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using NLog.Fluent;
using System.Data;
using HomeCook.Data.Enums;
using HomeCook.Data.CustomException;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth;
using HomeCook.Data.Models.CustomModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HomeCook.DTO.Pagination;
using System.Linq.Expressions;
using System.Web;
using HomeCook.DTO.ResetPassword;
using Microsoft.AspNetCore.WebUtilities;
using System.IO;

namespace HomeCook.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private AuthenticationSettings AuthSettings { get; }
        private IPasswordHasher<AppUser> PasswordHasher { get; }
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private readonly IConfigurationSection GoolgeSettings;
        private readonly IConfiguration Configuration;
        private IEmailService _emailService;
        private readonly IUserService _userService;

        public AuthService(DefaultDbContext context, SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager, 
            IPasswordHasher<AppUser> passwordHasher, 
            IMapper mapper,
            IConfiguration configuration,
            IEmailService emailService,
            IUserService userService,
            AuthenticationSettings authSettings) : base(context, mapper)
        {
            AuthSettings = authSettings;
            PasswordHasher = passwordHasher;

            _signInManager = signInManager;
            _userManager = userManager;
            Configuration = configuration;
            GoolgeSettings = Configuration.GetSection("Authentication:Google");
            _emailService = emailService;
            _userService = userService;

        }

        #region createJwtToken
        public async Task<AuthenticationResponse> CreateJwtTokens(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthSettings.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(AuthSettings.JwtExpireHours);

            IList<Claim> claims = await GetUserClaimsAsync(user);

            var token = new JwtSecurityToken(AuthSettings.JwtIssuer,
                AuthSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: creds);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token).ToString();
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddHours(AuthSettings.RefreshTokenExpireHours);
            await _userManager.UpdateAsync(user);

            return new AuthenticationResponse
            {
                JwtToken = tokenValue,
                RefreshToken = refreshToken,
                JwtTokenExpiryTime = DateTime.Now.AddHours(AuthSettings.JwtExpireHours)
                
            };
        }
        #endregion

        public async Task<AuthenticationResponse?> Login(LoginDto model)
        {
            var user = await _userService.FindUserAsync(model.Login);

            if (user is null)
            {
                return null;
            }
            await _signInManager.SignOutAsync();
            var passwordCheck = await _signInManager.UserManager.CheckPasswordAsync(user, model.Password);
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.remenberLogin, true);

            if (!result.Succeeded)
            {
                throw new AuthException(AuthException.InvalidLoginAttempt);
            }
            if (await _userManager.IsLockedOutAsync(user) && passwordCheck)
            {
                throw new UnauthorizedAccessException("Locked");
            }
            var tokens = await CreateJwtTokens(user);

            user.LastLogin = DateTime.Now;
            await _userManager.UpdateAsync(user);

            return tokens;
        }
        
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IList<Claim>> GetUserClaimsAsync(AppUser user)
        {
            return await _userManager.GetClaimsAsync(user);
        }
        public async Task<AppUser> Register(RegisterDto registerDto)
        {
            var newUser = new AppUser() { 
            Email = registerDto.Email,
            UserName = registerDto.Login,
            firstName = registerDto.FirstName,
            surname = registerDto.Surname,
            };
            try
            {
                var result = await _userManager.CreateAsync(newUser, registerDto.Password);
                if (!result.Succeeded)
                {
                    throw new AuthException(result.Errors.ToArray());
                }
                var claims = new[] {
                new Claim(ClaimTypes.Actor, ClaimType.User.ToString()),
                new Claim(ClaimTypes.Role, RoleType.User.ToString()),
                new Claim(ClaimTypes.NameIdentifier, registerDto.Email),
                new Claim("PublicId", newUser.Id)
            };
                result = await _userManager.AddClaimsAsync(newUser, claims);
                if (result.Succeeded)
                {
                    var emailVerificationCode = HttpUtility.UrlEncode(await _userManager.GenerateEmailConfirmationTokenAsync(newUser));
                    await _emailService.SendEmailConfirmationEmail(newUser, emailVerificationCode);
                    await _signInManager.SignInAsync(newUser, isPersistent: registerDto.RemenberLogin);

                    return newUser;
                }
                else
                {
                    await _userManager.DeleteAsync(newUser);
                    throw new Exception(AuthException.UserAlreadyExist);
                }
            }
            catch (Exception)
            {
                await _userManager.DeleteAsync(newUser);
                throw;
            }
            
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<AuthenticationResponse> Refresh(string refreshToken)
        {
            var user = _userManager.Users.FirstOrDefault(r => r.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.Now)
            {
                throw new AuthException(AuthException.InvalidRefreshToken);
            }
            var tokens = await CreateJwtTokens(user);
            return tokens;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { GoolgeSettings.GetSection("clientId").Value }
                };
                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                //log an exception
                return null;
            }
        }


        public AuthenticationProperties ConfigureExternalAuthProp(string provider, string redirectUrl)
        {
            return _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        }
        public async Task<ExternalLoginInfo> GetExternalLoginInfo()
        {
            return await _signInManager.GetExternalLoginInfoAsync();
            
        }
        public async Task<AuthenticationResponse> ExternalLogin(ExternalAuthDto externalAuth)
        {

            var payload = await VerifyGoogleToken(externalAuth);
            if (payload is null)
            {
                throw new AuthException(AuthException.InvalidLoginAttempt);
            }
            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);
            
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new AppUser { Email = payload.Email, UserName = payload.Email, surname = payload.FamilyName, firstName = payload.GivenName,EmailConfirmed = true };

                    await _userManager.CreateAsync(user);
                    
                    var claims = new[] {
                    new Claim(ClaimTypes.Actor, ClaimType.User.ToString()),
                    new Claim(ClaimTypes.Role, RoleType.User.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, payload.Email),
                    new Claim("PublicId", user.Id)
                       };
                    await _userManager.AddClaimsAsync(user, claims);

                    //prepare and send an email for the email confirmation
                    //await userManager.AddToRoleAsync(user, "Viewer");
                    await _userManager.AddLoginAsync(user, info);

                    #region update profile image
                    var profileImage = Context.ProfileImages.FirstOrDefault(x => x.UserId == user.Id);
                    try
                    {
                        var newPhoto = new ProfileImage()
                        {
                            Path = payload.Picture,
                            Name = "Google Profile Image",
                            UserId = user.Id,
                        };
                        if (profileImage is not null)
                        {
                            Context.ProfileImages.Remove(profileImage);
                        }
                        Context.ProfileImages.Add(newPhoto);
                        Context.SaveChanges();
                    }
                    catch (Exception)
                    {
                        throw new ImageException(ImageException.ProfileImageError);
                    }
                    #endregion
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }
            }
            if (user == null)
                throw new AuthException(AuthException.InvalidLoginAttempt);

            await _signInManager.SignInAsync(user, false);
            var tokens = await CreateJwtTokens(user);

            user.LastLogin = DateTime.Now;
            await _userManager.UpdateAsync(user);

            return tokens;

        }

        public async Task<IdentityResult> ConfirmEmailAddress(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed();

            return await _userManager.ConfirmEmailAsync(user, code);
        }

        public async Task<IdentityResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return IdentityResult.Failed();
            //URLEncode dlatego ze przy wysyłaniu na fronta jako query parameter sie psuje
            var forgorPasswordCode = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user));
            await _emailService.SendForgotPasswordEmail(user, forgorPasswordCode);
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> ResetPassword(ResetPasswordDto resetPassword)
        {
            var user = await _userManager.FindByIdAsync(resetPassword.UserId);
            if (user == null)
                return IdentityResult.Failed();

            return await _userManager.ResetPasswordAsync(user, resetPassword.ResetToken, resetPassword.NewPassword);
        }

        

        
    }
}
