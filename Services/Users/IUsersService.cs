using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ApiVending.Services.Users
{
    public interface IUsersServices
    {
         Task<List<UserDto>> GetUsersAsync(string? name);
         Task<UserDto> GetUserAsync(int id);
         Task<int> CreateUserAsync(CreateUserDto   dto);
         Task UpdateUserAsync(int id, UserDto dto);
         Task DeleteUserAsync(int id);
    }
}