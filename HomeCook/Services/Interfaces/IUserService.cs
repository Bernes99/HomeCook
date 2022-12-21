using HomeCook.Data.Models;
using HomeCook.Data.Models.CustomModels;
using HomeCook.DTO;
using HomeCook.DTO.Pagination;
using Microsoft.AspNetCore.Identity;

namespace HomeCook.Services.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> DeleteUser(string userId);
        Task<AppUser> FindUserAsync(string emailAddress);
        Task<AppUser> FindUserAsyncbyId(string userId);
        Task<UserDto> GetUserDto(string id);
        PaginationResult<UserDto> GetUsers(PaginationQuery query);
        Task<IdentityResult> UpdateUser(string userId, UserUpdateDto model);
    }
}