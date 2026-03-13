using LandInfoSystem.Data;
using LandInfoSystem.DTOs;
using LandInfoSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LandInfoSystem.Services
{
    public interface IFraudDetectionService
    {
        Task<FraudAnalysisResponseDto?> AnalyzePropertyAsync(int propertyId);
        Task<FraudAnalysisResponseDto?> GetFraudAnalysisAsync(int propertyId);
    }

    public class FraudDetectionService : IFraudDetectionService
    {
        private readonly ApplicationDbContext _context;

        public FraudDetectionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FraudAnalysisResponseDto?> GetFraudAnalysisAsync(int propertyId)
        {
            var existing = await _context.FraudAnalyses
                .FirstOrDefaultAsync(f => f.PropertyId == propertyId);

            if (existing == null)
                return await AnalyzePropertyAsync(propertyId);

            return MapToDto(existing);
        }

        public async Task<FraudAnalysisResponseDto?> AnalyzePropertyAsync(int propertyId)
        {
            var property = await _context.LandProperties
                .FirstOrDefaultAsync(p => p.PropertyId == propertyId);

            if (property == null) return null;

            int score = 0;
            bool isDuplicate = false;
            bool isPriceAnomaly = false;
            bool isSuspiciousSeller = false;

            // ── CHECK 1: Duplicate listing ─────────────────────────────────
            // Same city + pincode listed by a DIFFERENT seller
            var duplicateExists = await _context.LandProperties
                .AnyAsync(p =>
                    p.PropertyId != propertyId &&
                    p.City.ToLower() == property.City.ToLower() &&
                    p.PinCode == property.PinCode &&
                    p.SellerId != property.SellerId &&
                    p.IsAvailable);

            if (duplicateExists)
            {
                isDuplicate = true;
                score += 40;
            }

            // ── CHECK 2: Price anomaly ─────────────────────────────────────
            // Price < 50% of average price in the same city
            var cityProperties = await _context.LandProperties
                .Where(p =>
                    p.PropertyId != propertyId &&
                    p.City.ToLower() == property.City.ToLower() &&
                    p.Price > 0)
                .ToListAsync();

            if (cityProperties.Count >= 2)
            {
                var avgPrice = cityProperties.Average(p => (double)p.Price);
                if ((double)property.Price < avgPrice * 0.50)
                {
                    isPriceAnomaly = true;
                    score += 35;
                }
            }

            // ── CHECK 3: Suspicious seller activity ───────────────────────
            // Seller posted >5 listings in the last 30 days
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var recentListingCount = await _context.LandProperties
                .CountAsync(p =>
                    p.SellerId == property.SellerId &&
                    p.CreatedDate >= thirtyDaysAgo);

            if (recentListingCount > 5)
            {
                isSuspiciousSeller = true;
                score += 25;
            }

            // ── Clamp score ───────────────────────────────────────────────
            score = Math.Min(score, 100);

            // ── Risk level ────────────────────────────────────────────────
            string riskLevel = score <= 30 ? "Low" : score <= 60 ? "Medium" : "High";

            // ── Warning message ───────────────────────────────────────────
            string? warning = null;
            if (score > 30)
            {
                var reasons = new System.Collections.Generic.List<string>();
                if (isDuplicate) reasons.Add("duplicate location detected");
                if (isPriceAnomaly) reasons.Add("price is unusually low for this area");
                if (isSuspiciousSeller) reasons.Add("seller has unusually high recent activity");
                warning = $"⚠️ Risky Property — {string.Join(", ", reasons)}. Please verify documents before purchase.";
            }

            // ── Upsert fraud record ───────────────────────────────────────
            var existing = await _context.FraudAnalyses
                .FirstOrDefaultAsync(f => f.PropertyId == propertyId);

            if (existing == null)
            {
                existing = new FraudAnalysis { PropertyId = propertyId };
                _context.FraudAnalyses.Add(existing);
            }

            existing.FraudScore = score;
            existing.RiskLevel = riskLevel;
            existing.IsDuplicate = isDuplicate;
            existing.IsPriceAnomaly = isPriceAnomaly;
            existing.IsSuspiciousSeller = isSuspiciousSeller;
            existing.WarningMessage = warning;
            existing.AnalyzedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(existing);
        }

        private static FraudAnalysisResponseDto MapToDto(FraudAnalysis f) => new()
        {
            PropertyId        = f.PropertyId,
            FraudScore        = f.FraudScore,
            RiskLevel         = f.RiskLevel,
            IsDuplicate       = f.IsDuplicate,
            IsPriceAnomaly    = f.IsPriceAnomaly,
            IsSuspiciousSeller = f.IsSuspiciousSeller,
            WarningMessage    = f.WarningMessage,
            AnalyzedAt        = f.AnalyzedAt
        };
    }
}
