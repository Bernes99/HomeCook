using HomeCook.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace HomeCook.Services
{
    public class IdentityManager
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public IdentityManager( UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            userManager = userManager;
            signInManager = signInManager;
        }

        public async Task<AppUser> LoginAsync(String emailAddress, String password)
        {
            var user = await FindUserAsync(emailAddress);
            if (null != user)
            {
                var passwordCheck = await signInManager.UserManager.CheckPasswordAsync(user, password);
                var result = await signInManager.PasswordSignInAsync(user, password, true, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    return user;
                }
                if (await userManager.IsLockedOutAsync(user) && passwordCheck)
                {
                    throw new UnauthorizedAccessException("Locked");
                }
                if (result.RequiresTwoFactor)
                    return user;
                else
                {
                    throw new UnauthorizedAccessException();
                }
            }
            else
            {
                throw new UnauthorizedAccessException();
            }
        }

        public async Task<AppUser> FindUserAsync(string emailAddress)
        {
            return await userManager.FindByEmailAsync(emailAddress);
        }
    }
}
