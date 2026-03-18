using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiVending.DTO;
using ApiVending.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiVending.Services.Users
{
    public class UserService(VendingSystemDbContext context) : IUsersServices
    {
        private readonly VendingSystemDbContext _context = context;
       async public Task<int>  CreateUserAsync(CreateUserDto  dto)
        {
            if (await _context.Users.AnyAsync(x => x.Username == dto.Username))
            throw new InvalidOperationException("Username существует");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Fio = dto.Fio,
            Email = dto.Email,
            Phone = dto.Phone,
            RoleId = dto.RoleId,
            CompanyId = dto.CompanyId,
            IsActive = dto.IsActive ?? true,
            PhotoUrl = dto.PhotoUrl,
            CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user.Id;

        }

       async public Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id) ?? throw new KeyNotFoundException($"юзер с таким айди не найден");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

       async public Task<UserDto> GetUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id) ?? throw new KeyNotFoundException($"юзер с таким id {id} не найлен");
            return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Fio = user.Fio,
            Email = user.Email,
            Phone = user.Phone,
            RoleId = user.RoleId,
            CompanyId = user.CompanyId,
            IsActive = user.IsActive,
            PhotoUrl = user.PhotoUrl
        };

        }

       async public Task<List<UserDto>> GetUsersAsync(string? name)
        {
            var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(name))      
            query = query.Where(x => x.Fio.Contains(name));
        

        return await query
            .Select(x => new UserDto
            {
                Id = x.Id,
                Username = x.Username,
                Fio = x.Fio,
                Email = x.Email,
                Phone = x.Phone,
                RoleId = x.RoleId,
                CompanyId = x.CompanyId,
                IsActive = x.IsActive,
                PhotoUrl = x.PhotoUrl
            }).ToListAsync();
        }

       async public Task UpdateUserAsync(int id, UserDto dto)
        {
            var user = await _context.Users.FindAsync(id) ?? throw new KeyNotFoundException($"юзер с таким айди не найден");
            if (await _context.Users.AnyAsync(x => x.Username == dto.Username && x.Id != id))
        throw new InvalidOperationException("Username существует");
        user.Username = dto.Username;
        user.Fio = dto.Fio;
        user.Email = dto.Email;
        user.Phone = dto.Phone;
        user.RoleId = dto.RoleId;
        user.CompanyId = dto.CompanyId;
        user.IsActive = dto.IsActive;
        user.PhotoUrl = dto.PhotoUrl;

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();
        }
    }
}