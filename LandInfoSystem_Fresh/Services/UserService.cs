using Microsoft.EntityFrameworkCore;
using LandInfoSystem.Data;
using LandInfoSystem.Models;
using LandInfoSystem.DTOs;

namespace LandInfoSystem.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task<UserResponseDto> GetUserByUsernameAsync(string username);
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<List<UserResponseDto>> GetSellersByTypeAsync();
        Task<UserResponseDto> UpdateUserAsync(int id, UserResponseDto dto);
        Task<bool> DeactivateUserAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return null;

            return MapToDto(user);
        }

        public async Task<UserResponseDto> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

            return MapToDto(user);
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(u => MapToDto(u)).ToList();
        }

        public async Task<List<UserResponseDto>> GetSellersByTypeAsync()
        {
            var sellers = await _context.Users
                .Where(u => u.UserType == "Seller" && u.IsActive)
                .ToListAsync();

            return sellers.Select(u => MapToDto(u)).ToList();
        }

        public async Task<UserResponseDto> UpdateUserAsync(int id, UserResponseDto dto)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return null;

            user.FirstName = dto.FirstName ?? user.FirstName;
            user.LastName = dto.LastName ?? user.LastName;
            user.Phone = dto.Phone ?? user.Phone;
            user.Address = dto.Address ?? user.Address;

            await _context.SaveChangesAsync();

            return MapToDto(user);
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return false;

            user.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }

        private UserResponseDto MapToDto(User user)
        {
            return new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = user.UserType,
                Phone = user.Phone,
                Address = user.Address,
                CreatedDate = user.CreatedDate,
                IsActive = user.IsActive
            };
        }
    }
}
