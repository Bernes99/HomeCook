using AutoMapper;
using HomeCook.Data.CustomException;
using HomeCook.Data.Enums;
using HomeCook.Data.Models.CustomModels;
using HomeCook.Data.Models;
using HomeCook.DTO.Pagination;
using HomeCook.DTO;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;
using HomeCook.Data;
using HomeCook.Services.Interfaces;
using HomeCook.Data.Extensions;

namespace HomeCook.Services
{
    public class UserService : BaseService, IUserService
    {
        private UserManager<AppUser> _userManager;

        public UserService(DefaultDbContext context,
            UserManager<AppUser> userManager,
            IMapper mapper) : base(context, mapper)
        {
            _userManager = userManager;
        }

        public async Task<AppUser> FindUserAsync(string emailAddress)
        {
            return await _userManager.FindByEmailAsync(emailAddress) ?? await _userManager.FindByNameAsync(emailAddress);
        }
        public async Task<AppUser> FindUserAsyncbyId(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public PaginationResult<UserDto> GetUsers(UserPaginationQuery query)
        {
            string searchPhrase = "";
            if (query is not null)
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
                s.NormalizedEmail.Contains(searchPhrase)) && !s.IsDeleted);

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

        public async Task<UserDto> GetUserDto(string id)
        {
            var user = Context.Users.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            if (user is null )
            {
                throw new NullReferenceException(); //TODO
            }
            var userDto = Mapper.Map<UserDto>(user);

            return userDto;
        }

        public async Task<IdentityResult> DeleteUser(string userId)
        {
            var user = await FindUserAsyncbyId(userId);

            if (user is null)
            {
                throw new AuthException(AuthException.UserDoesNotExist);
            }
            user.IsDeleted = true;
            return await _userManager.UpdateAsync(user);

        }
        public async Task<IdentityResult> UpdateUser(string userId, UserUpdateDto model)
        {
            var user = await FindUserAsyncbyId(userId);

            if (user is null)
            {
                throw new AuthException(AuthException.UserDoesNotExist);
            }
            if (model is null)
            {
                throw new AuthException(AuthException.BadRequest);
            }

            user.firstName = model.FirstName ?? user.firstName ?? null;
            user.surname = model.Surname ?? user.surname ?? null;
            user.UserName = model.UserName ?? user.UserName ?? null;
            user.Email = model.Email ?? user.Email ?? null;

            if (string.IsNullOrEmpty(model.Email))
            {
                await _userManager.UpdateNormalizedEmailAsync(user);
            }
            if (string.IsNullOrEmpty(model.UserName))
            {
                await _userManager.UpdateNormalizedUserNameAsync(user);
            }

            return await _userManager.UpdateAsync(user);

        }
    }
}
