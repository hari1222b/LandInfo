using Microsoft.AspNetCore.Mvc;

namespace LandInfoSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        [HttpPost("chat")]
        public ActionResult<object> Chat([FromBody] ChatRequest request)
        {
            try
            {
                string userMessage = request.Message.ToLower();
                string response = GetResponse(userMessage);

                return Ok(new { success = true, message = response });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
        }

        private string GetResponse(string message)
        {
            if (message.Contains("price") || message.Contains("cost"))
                return "💰 Land prices in Coimbatore range from ₹5-15 lakhs per acre. Premium locations like Avinashi Road cost more. Contact sellers directly for best rates!";
            
            if (message.Contains("invest"))
                return "📊 Investment tips: Check location development, proximity to highways, water availability, and past price trends. Start with research!";
            
            if (message.Contains("buy") || message.Contains("purchase"))
                return "🏠 To buy: Browse properties, contact sellers, negotiate, complete legal documentation. No broker needed - save commission!";
            
            if (message.Contains("sell") || message.Contains("post"))
                return "📢 To sell: Register as seller, add property details, set price, wait for inquiries. Quick and easy process!";
            
            if (message.Contains("location") || message.Contains("area"))
                return "📍 Popular areas: Avinashi Road (commercial), SIPCOT (industrial), Kanjikode (residential). Choose based on your needs!";
            
            if (message.Contains("document"))
                return "📋 Required docs: Title deed, sale agreement, tax receipts, municipal certificate. Consult a lawyer!";
            
            if (message.Contains("loan") || message.Contains("finance"))
                return "💳 Get bank loans up to 80% of property value. Check eligibility with your bank!";
            
            if (message.Contains("time") || message.Contains("when"))
                return "⏰ Best time to invest: When you find good location at right price. Coimbatore is growing rapidly - good time now!";
            
            if (message.Contains("broker") || message.Contains("commission"))
                return "✋ No brokers here! Direct seller-buyer connection saves you 5-10% commission. Transparent pricing!";

            return "👋 Hi! Ask me about prices, locations, buying/selling tips, investments, or documents. What would you like to know?";
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}