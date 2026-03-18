using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using ApiVending.Models;
using ApiVending.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiVending.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IJwtService jwtService, VendingSystemDbContext context) : ControllerBase
    {
        public static class TokenBlacklist
        {
            private static readonly HashSet<string> _blacklisted = [];
            public static void Add(string token) => _blacklisted.Add(token);
            public static bool IsBlacklisted(string token) => _blacklisted.Contains(token);
        }
        
        private readonly IJwtService _jwtService = jwtService;
        private readonly VendingSystemDbContext _context = context;

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null) return Unauthorized("Неверный логин или пароль");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Неверный логин или пароль");
        
            var token = _jwtService.GenerateToken(user);
            return Ok(new {token,username = user.Username});
        } 
        [HttpPost("refresh-token")]
public async Task<IActionResult> RefreshToken([FromBody] string oldToken)
{
    var principal = _jwtService.ValidateToken(oldToken);
    if (principal == null)
        return Unauthorized("Токен недействителен");

    var username = principal.Identity?.Name;
    if (username == null)
        return Unauthorized("Токен недействителен");

    var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);

    if (user == null)
        return Unauthorized("Пользователь не найден");

    TokenBlacklist.Add(oldToken);
    var newToken = _jwtService.GenerateToken(user);
    return Ok(new { token = newToken, username = user.Username });
}

[HttpPost("logout")]
[Authorize]
public IActionResult Logout()
{
    var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

    if (!string.IsNullOrEmpty(token))
        TokenBlacklist.Add(token);

    return Ok(new { message = "Выход выполнен" });
}
    }
}