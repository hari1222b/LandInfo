using LandInfoSystem.Data;
using LandInfoSystem.Models;
using LandInfoSystem.DTOs;

namespace LandInfoSystem.Services
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(UserRegisterDto dto);
        Task<LoginResponseDto> LoginAsync(UserLoginDto dto);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterResponseDto> RegisterAsync(UserRegisterDto dto)
        {
            try
            {
                // Check if user already exists
                if (_context.Users.Any(u => u.Username == dto.Username))
                {
                    return new RegisterResponseDto
                    {
                        Success = false,
                        Message = "Username already exists"
                    };
                }

                if (_context.Users.Any(u => u.Email == dto.Email))
                {
                    return new RegisterResponseDto
                    {
                        Success = false,
                        Message = "Email already registered"
                    };
                }

                // Create new user
                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    Password = HashPassword(dto.Password),
                    FirstName = dto.FirstName ?? "",
                    LastName = dto.LastName ?? "",
                    Phone = dto.Phone ?? "",
                    Address = dto.Address ?? "",
                    UserType = dto.UserType ?? "Buyer",
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new RegisterResponseDto
                {
                    Success = true,
                    Message = "Registration successful."
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<LoginResponseDto> LoginAsync(UserLoginDto dto)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == dto.Username);

                if (user == null || !VerifyPassword(dto.Password, user.Password))
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                if (!user.IsActive)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "User account is inactive"
                    };
                }

                // Log user right in
                return new LoginResponseDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    UserType = user.UserType,
                    Success = true,
                    Message = "Login successful"
                };
            }
            catch (Exception ex)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }



        public string HashPassword(string password)
        {
            // Using BCrypt for password hashing (simple implementation)
            // In production, use BCrypt.Net-Next NuGet package
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            // Verify hashed password
            // In production, use BCrypt.Net-Next NuGet package
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
