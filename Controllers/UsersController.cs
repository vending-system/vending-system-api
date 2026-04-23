using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using ApiVending.Models;
using ApiVending.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiVending.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController(IUsersServices usersServices) : ControllerBase
    {
    private readonly IUsersServices _usersServices = usersServices;
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetUsers(string? name)
    {
        var users = await _usersServices.GetUsersAsync(name);

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var dto = await _usersServices.GetUserAsync(id);

        return Ok(dto);
    }

     [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto  dto)
    {
        var user = await _usersServices.CreateUserAsync(dto);

        return Ok(new { user });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserDto dto)
    {
        await _usersServices.UpdateUserAsync(id,dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _usersServices.DeleteUserAsync(id);
        return NoContent();
    }
    }
}