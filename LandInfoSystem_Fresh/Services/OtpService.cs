using Microsoft.Extensions.Caching.Memory;
using System;

namespace LandInfoSystem.Services
{
    public interface IOtpService
    {
        string GenerateOtp(string key);
        bool VerifyOtp(string key, string otp);
        void ClearOtp(string key);
    }

    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly Random _random;

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
            _random = new Random();
        }

        public string GenerateOtp(string key)
        {
            // Generate a 6-digit OTP
            string otp = _random.Next(100000, 999999).ToString();

            // Store in cache with a 5-minute expiration
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(key, otp, cacheEntryOptions);

            return otp;
        }

        public bool VerifyOtp(string key, string otp)
        {
            if (_cache.TryGetValue(key, out string cachedOtp))
            {
                return cachedOtp == otp;
            }
            return false;
        }

        public void ClearOtp(string key)
        {
            _cache.Remove(key);
        }
    }
}
