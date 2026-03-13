using Microsoft.EntityFrameworkCore;
using LandInfoSystem.Data;
using LandInfoSystem.Models;
using LandInfoSystem.DTOs;

namespace LandInfoSystem.Services
{
    public interface IPropertyService
    {
        Task<List<PropertyResponseDto>> GetAllPropertiesAsync();
        Task<List<PropertyResponseDto>> GetAvailablePropertiesAsync();
        Task<PropertyResponseDto> GetPropertyByIdAsync(int id);
        Task<List<PropertyResponseDto>> GetPropertiesBySeller(int sellerId);
        Task<PropertyResponseDto> CreatePropertyAsync(CreatePropertyDto dto);
        Task<PropertyResponseDto> UpdatePropertyAsync(int id, UpdatePropertyDto dto);
        Task<bool> DeletePropertyAsync(int id);
        Task<List<PropertyResponseDto>> SearchPropertiesAsync(string city, decimal? minPrice, decimal? maxPrice);
        Task<PropertyResponseDto> UpdatePropertyBoundaryAsync(int id, string boundaryJson);
    }

    public class PropertyService : IPropertyService
    {
        private readonly ApplicationDbContext _context;

        public PropertyService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PropertyResponseDto>> GetAllPropertiesAsync()
        {
            var properties = await _context.LandProperties
                .Include(p => p.Seller)
                .ToListAsync();

            return properties.Select(p => MapToDto(p)).ToList();
        }

        public async Task<List<PropertyResponseDto>> GetAvailablePropertiesAsync()
        {
            var properties = await _context.LandProperties
                .Include(p => p.Seller)
                .Where(p => p.IsAvailable)
                .ToListAsync();

            return properties.Select(p => MapToDto(p)).ToList();
        }

        public async Task<PropertyResponseDto> GetPropertyByIdAsync(int id)
        {
            var property = await _context.LandProperties
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            if (property == null)
                return null;

            return MapToDto(property);
        }

        public async Task<List<PropertyResponseDto>> GetPropertiesBySeller(int sellerId)
        {
            var properties = await _context.LandProperties
                .Include(p => p.Seller)
                .Where(p => p.SellerId == sellerId)
                .ToListAsync();

            return properties.Select(p => MapToDto(p)).ToList();
        }

        public async Task<PropertyResponseDto> CreatePropertyAsync(CreatePropertyDto dto)
        {
            var property = new LandProperty
            {
                SellerId = dto.SellerId,
                Title = dto.Title,
                Description = dto.Description,
                Area = dto.Area,
                Price = dto.Price,
                Location = dto.Location,
                City = dto.City,
                State = dto.State,
                PinCode = dto.PinCode,
                ImageUrl = dto.ImageUrl ?? "",
                IsAvailable = true,
                CreatedDate = DateTime.Now
            };

            _context.LandProperties.Add(property);
            await _context.SaveChangesAsync();

            property = await _context.LandProperties
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.PropertyId == property.PropertyId);

            return MapToDto(property);
        }

        public async Task<PropertyResponseDto> UpdatePropertyAsync(int id, UpdatePropertyDto dto)
        {
            var property = await _context.LandProperties.FindAsync(id);

            if (property == null)
                return null;

            property.Title = dto.Title ?? property.Title;
            property.Description = dto.Description ?? property.Description;
            property.Area = dto.Area > 0 ? dto.Area : property.Area;
            property.Price = dto.Price > 0 ? dto.Price : property.Price;
            property.Location = dto.Location ?? property.Location;
            property.City = dto.City ?? property.City;
            property.State = dto.State ?? property.State;
            property.PinCode = dto.PinCode ?? property.PinCode;
            property.ImageUrl = dto.ImageUrl ?? property.ImageUrl;
            property.BoundaryJson = dto.BoundaryJson ?? property.BoundaryJson;
            property.IsAvailable = dto.IsAvailable;
            property.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            property = await _context.LandProperties
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            return MapToDto(property);
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            var property = await _context.LandProperties.FindAsync(id);

            if (property == null)
                return false;

            _context.LandProperties.Remove(property);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<PropertyResponseDto> UpdatePropertyBoundaryAsync(int id, string boundaryJson)
        {
            var property = await _context.LandProperties.FindAsync(id);
            if (property == null) return null;

            property.BoundaryJson = boundaryJson;
            property.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            
            property = await _context.LandProperties
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(p => p.PropertyId == id);

            return MapToDto(property);
        }

        public async Task<List<PropertyResponseDto>> SearchPropertiesAsync(string city, decimal? minPrice, decimal? maxPrice)
        {
            var query = _context.LandProperties
                .Include(p => p.Seller)
                .Where(p => p.IsAvailable)
                .AsQueryable();

            if (!string.IsNullOrEmpty(city))
            {
                query = query.Where(p => p.City.ToLower().Contains(city.ToLower()) || 
                                        p.Location.ToLower().Contains(city.ToLower()));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var properties = await query.ToListAsync();

            return properties.Select(p => MapToDto(p)).ToList();
        }

        private PropertyResponseDto MapToDto(LandProperty property)
        {
            return new PropertyResponseDto
            {
                PropertyId = property.PropertyId,
                SellerId = property.SellerId,
                Title = property.Title,
                Description = property.Description,
                Area = property.Area,
                Price = property.Price,
                Location = property.Location,
                City = property.City,
                State = property.State,
                PinCode = property.PinCode,
                ImageUrl = property.ImageUrl,
                BoundaryJson = property.BoundaryJson,
                IsAvailable = property.IsAvailable,
                CreatedDate = property.CreatedDate,
                UpdatedDate = property.UpdatedDate.GetValueOrDefault(DateTime.Now),
                Seller = property.Seller != null ? new UserResponseDto
                {
                    UserId = property.Seller.UserId,
                    Username = property.Seller.Username,
                    Email = property.Seller.Email,
                    FirstName = property.Seller.FirstName,
                    LastName = property.Seller.LastName,
                    UserType = property.Seller.UserType,
                    Phone = property.Seller.Phone,
                    Address = property.Seller.Address,
                    CreatedDate = property.Seller.CreatedDate,
                    IsActive = property.Seller.IsActive
                } : null
            };
        }
    }
}
