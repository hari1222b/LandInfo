using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LandInfoSystem.DTOs;

namespace LandInfoSystem.Services
{
    public interface ILandPredictionService
    {
        Task<LandPredictionResponseDto> PredictAsync(LandPredictionRequestDto request);
    }

    public class LandPredictionService : ILandPredictionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _mlServiceUrl = "http://127.0.0.1:8000/predict";

        public LandPredictionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LandPredictionResponseDto> PredictAsync(LandPredictionRequestDto request)
        {
            try
            {
                // Map the C# DTO to the Python FastAPI expected JSON schema
                var payload = new
                {
                    latitude = request.Latitude,
                    longitude = request.Longitude,
                    land_size = request.LandSize,
                    road_access = request.RoadAccess,
                    water_facility = request.WaterFacility,
                    school_distance = request.DistanceSchool,
                    hospital_distance = request.DistanceHospital
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(_mlServiceUrl, content);
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"ML Service returned {response.StatusCode}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                
                // The python backend returns: {"predicted_price": 1234.5, "trend_data": [{"month": "Jan", "price": 100}, ...]}
                // We need to parse this into our C# DTO
                
                using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                JsonElement root = doc.RootElement;

                var result = new LandPredictionResponseDto();
                
                if (root.TryGetProperty("predicted_price", out JsonElement priceEl))
                {
                    result.PredictedPrice = priceEl.GetDecimal();
                }

                result.PriceTrend = new List<PriceTrendPoint>();
                
                if (root.TryGetProperty("trend_data", out JsonElement trendEl) && trendEl.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in trendEl.EnumerateArray())
                    {
                        result.PriceTrend.Add(new PriceTrendPoint
                        {
                            Month = item.GetProperty("month").GetString() ?? "",
                            Price = item.GetProperty("price").GetDecimal()
                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log exception in a real app
                throw new Exception($"Failed to predict price from AI Model: {ex.Message}");
            }
        }
    }
}
