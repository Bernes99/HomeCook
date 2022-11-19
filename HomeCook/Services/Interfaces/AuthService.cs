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

namespace HomeCook.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private AuthenticationSettings AuthSettings { get; }
        private IPasswordHasher<AppUser> PasswordHasher { get; }
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private readonly IConfigurationSection GoolgeSettings;
        private readonly IConfiguration Configuration;
        public AuthService(DefaultDbContext context, SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager, 
            IPasswordHasher<AppUser> passwordHasher, 
            IMapper mapper,
            IConfiguration configuration,
            AuthenticationSettings authSettings) : base(context, mapper)
        {
            AuthSettings = authSettings;
            PasswordHasher = passwordHasher;

            this.signInManager = signInManager;
            this.userManager = userManager;
            Configuration = configuration;
            GoolgeSettings = Configuration.GetSection("Authentication:Google");

        }

        #region createJwtToken
        public async Task<AuthenticationResponse> CreateJwtTokens(AppUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthSettings.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(AuthSettings.JwtExpireHours);

            IList<Claim> claims = await GetUserClaimsAsync(user);
            //var claims = new List<Claim>()
            //{
            //    new Claim(ClaimTypes.Name, $"{user.UserName}"),
            //    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //    new Claim(JwtRegisteredClaimNames.Email, user.Email)
            //};

            var token = new JwtSecurityToken(AuthSettings.JwtIssuer,
                AuthSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: creds);

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token).ToString();
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddHours(AuthSettings.RefreshTokenExpireHours);
            await userManager.UpdateAsync(user);

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
            var user = await FindUserAsync(model.Login);

            if (user is null)
            {
                return null;
            }
            await signInManager.SignOutAsync();
            var passwordCheck = await signInManager.UserManager.CheckPasswordAsync(user, model.Password);
            var result = await signInManager.PasswordSignInAsync(user, model.Password, model.remenberLogin, true);

            if (!result.Succeeded)
            {
                return null;
            }
            if (await userManager.IsLockedOutAsync(user) && passwordCheck)
            {
                throw new UnauthorizedAccessException("Locked");
            }
            var tokens = await CreateJwtTokens(user);

            return tokens;
        }
        
        public async Task Logout()
        {
            await signInManager.SignOutAsync();
        }
        public async Task<AppUser> FindUserAsync(string emailAddress)
        {
            return await userManager.FindByEmailAsync(emailAddress) ?? await userManager.FindByNameAsync(emailAddress); ;
        }
        public async Task<IList<Claim>> GetUserClaimsAsync(AppUser user)
        {
            return await userManager.GetClaimsAsync(user);
        }
        public async Task<AppUser> Register(RegisterDto registerDto)
        {
            var newUser = new AppUser() { 
            Email = registerDto.Email,
            UserName = registerDto.Login,
            firstName = registerDto.FirstName,
            surname = registerDto.Surname,
            };

            var result = await userManager.CreateAsync(newUser, registerDto.Password);

            var claims = new[] {
                    new Claim(ClaimTypes.Actor, ClaimType.User.ToString()),
                    new Claim(ClaimTypes.Role, RoleType.User.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, registerDto.Email),
                    new Claim("PublicId", newUser.Id)
            };
            result = await userManager.AddClaimsAsync(newUser, claims);
            if (result.Succeeded)
            {
                await signInManager.SignInAsync(newUser, isPersistent: registerDto.RemenberLogin);

                return newUser;
            }
            else
            {
                await userManager.DeleteAsync(newUser);
                //throw new AuthException(AuthException.UserAlreadyExist);
                throw new Exception(AuthException.UserAlreadyExist);
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
        public async void test()
        {
            var test2 = Context.Users.ToList();
            var test = Context.Products.ToList();

            //var test3 = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
        }

        public async Task<AuthenticationResponse> Refresh(string refreshToken)
        {
            var user = userManager.Users.FirstOrDefault(r => r.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpiryTime < DateTime.Now)
            {
                throw new AuthException(AuthException.InvalidRefreshToken);
            }
            var tokens = await CreateJwtTokens(user);
            return tokens;

            //var tokenValidationParameters = new TokenValidationParameters
            //{
            //    ValidateAudience = false,
            //    ValidateIssuer = false,
            //    ValidateIssuerSigningKey = true,
            //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthSettings.JwtKey)),
            //    ValidateLifetime = false
            //};

            //var tokenHandler = new JwtSecurityTokenHandler();
            //SecurityToken securityToken;
            //var principal = tokenHandler.ValidateToken(authenticationResponse.JwtToken, tokenValidationParameters, out securityToken);
            //var jwtSecurityToken = securityToken as JwtSecurityToken;
            //if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    throw new SecurityTokenException("Invalid token");
            //}

            //var email = principal.Claims.FirstOrDefault(c => c.Type == "Email");
            //var user = Context.Users.FirstOrDefault(u => u.Email == email.Value);

            //if (user == null || user.RefreshToken != authenticationResponse.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            //{
            //    throw new Exception("Invalid client request");
            //}

            //var result = CreateJwtToken(user);

            //return new LoginResponse
            //{
            //    JwtToken = result.JwtToken,
            //    RefreshToken = result.RefreshToken,
            //    JwtTokenExpiryTime = DateTime.Now.AddDays(AuthSettings.JwtExpireDays)
            //};
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
            return signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        }
        public async Task<ExternalLoginInfo> GetExternalLoginInfo()
        {
            return await signInManager.GetExternalLoginInfoAsync();
            
        }
        public async Task<AuthenticationResponse> ExternalLogin(ExternalAuthDto externalAuth)
        {

            var payload = await VerifyGoogleToken(externalAuth);
            if (payload is null)
            {
                throw new AuthException(AuthException.InvalidLoginAttempt);
            }
            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);

            var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new AppUser { Email = payload.Email, UserName = payload.Email, surname = payload.FamilyName, firstName = payload.GivenName };

                    await userManager.CreateAsync(user);

                    var claims = new[] {
                    new Claim(ClaimTypes.Actor, ClaimType.User.ToString()),
                    new Claim(ClaimTypes.Role, RoleType.User.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, payload.Email),
                    new Claim("PublicId", user.Id)
                       };
                     await userManager.AddClaimsAsync(user, claims);

                    //prepare and send an email for the email confirmation
                    //await userManager.AddToRoleAsync(user, "Viewer");
                    await userManager.AddLoginAsync(user, info);
                }
                else
                {
                    await userManager.AddLoginAsync(user, info);
                }
            }
            if (user == null)
                throw new AuthException(AuthException.InvalidLoginAttempt);

            await signInManager.SignInAsync(user, false);
            var tokens = await CreateJwtTokens(user);
            return tokens;

            //await userManager.SetAuthenticationTokenAsync(
            //        user,
            //        TokenOptions.DefaultProvider,
            //        "X-Refresh-Token",
            //        tokens.RefreshToken);

            //var signinResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            //var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            //var user = await userManager.FindByEmailAsync(email);
            //var claims = await GetUserClaimsAsync(user);

            //if (signinResult.Succeeded)
            //{
            //    var tokens = await CreateJwtTokens(user);
            //    //var jwtResult = await jwtAuthManager.GenerateTokens(user, claims, DateTime.UtcNow);

            //    await userManager.SetAuthenticationTokenAsync(
            //        user,
            //        TokenOptions.DefaultProvider,
            //        "X-Refresh-Token",
            //        tokens.RefreshToken);

            //    //loginResult = new LoginResultViewModel()
            //    //{
            //    //    User = new UserViewModel()
            //    //    {
            //    //        Email = email,
            //    //        AccessToken = jwtResult.AccessToken,
            //    //        RefreshToken = jwtResult.RefreshToken,
            //    //        FirstName = user.FirstName,
            //    //        LastName = user.LastName,
            //    //        Phone = user.PhoneNumber,
            //    //        UserId = user.Id
            //    //    }
            //    //};

            //    return tokens;
            //}

            //if ( !String.IsNullOrEmpty(email))
            //{
            //    if (user == null)
            //    {
            //        user = new AppUser()
            //        {
            //            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
            //            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
            //        };
            //        await userManager.CreateAsync(user);
            //    }

            //    await userManager.AddLoginAsync(user, info);
            //    await signInManager.SignInAsync(user, false);
            //    var tokens = await CreateJwtTokens(user);
            //    //var jwtResult = await jwtAuthManager.GenerateTokens(user, claims, DateTime.UtcNow);

            //    //sucess
            //    await userManager.SetAuthenticationTokenAsync(
            //        user,
            //        TokenOptions.DefaultProvider,
            //        "X-Refresh-Token",
            //        tokens.RefreshToken);

            //    //loginResult = new LoginResultViewModel()
            //    //{
            //    //    User = new UserViewModel()
            //    //    {
            //    //        Email = email,
            //    //        AccessToken = jwtResult.AccessToken,
            //    //        RefreshToken = jwtResult.RefreshToken,
            //    //        FirstName = user.FirstName,
            //    //        LastName = user.LastName,
            //    //        Phone = user.PhoneNumber,
            //    //        UserId = user.Id
            //    //    }
            //    //};

            //    return tokens;
            //}

            //return null;
        }

        public PaginationResult<UserDto> GetUsers(PaginationQuery query)
        {
            string searchPhrase = "";
            if (query is not null )
            {
                if (query.SearchPhrase is not null)
                {
                    searchPhrase = query.SearchPhrase.ToUpper();
                }
                if (query.SortBy is not null)
                {
                    query.SortBy = query.SortBy.ToUpper();
                }
                
            }
            var users = Context.Users.Where(s => query.SearchPhrase == null || (s.firstName.ToUpper().Contains(searchPhrase) || s.surname.ToUpper().Contains(searchPhrase) ||
                s.NormalizedEmail.Contains(searchPhrase)));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<AppUser, object>>>
                {
                    { nameof(AppUser.firstName).ToUpper(), u => u.firstName },
                    { nameof(AppUser.surname).ToUpper(), u => u.surname },
                    { nameof(AppUser.Email).ToUpper(), u => u.Email },
                    { nameof(AppUser.LastLogin).ToUpper(), u => u.LastLogin },
                };
                var selectedColumn = columnsSelectors[query.SortBy];

                users = query.SortDirection == SortDirection.ASC ? users.OrderBy(selectedColumn) : users.OrderByDescending(selectedColumn);
            }

            var pagedUsers = users
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize);

            var usersDto = Mapper.Map<List<UserDto>>(pagedUsers);

            var usersCount = users.Count();
            var result = new PaginationResult<UserDto>(usersDto, usersCount, query.PageSize, query.PageNumber);
            return result;
        }
    }
}
