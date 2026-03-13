using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace LandInfoSystem.Services
{
    public interface ISmsService
    {
        Task SendSmsAsync(string toPhone, string message);
    }

    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;

        public SmsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendSmsAsync(string toPhone, string message)
        {
            var accountSid = _configuration["TwilioSettings:AccountSid"] ?? "";
            var authToken = _configuration["TwilioSettings:AuthToken"] ?? "";
            var fromPhone = _configuration["TwilioSettings:FromPhoneNumber"] ?? "";

            // Do not send if unconfigured
            if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromPhone))
            {
                return;
            }

            TwilioClient.Init(accountSid, authToken);

            // Ensure the phone number is correctly formatted for Twilio. E.g. "+91xxxxxxxxxx"
            if (!toPhone.StartsWith("+"))
            {
                toPhone = "+91" + toPhone; // Defaulting to India country code for this example
            }

            await MessageResource.CreateAsync(
                to: new PhoneNumber(toPhone),
                from: new PhoneNumber(fromPhone),
                body: message
            );
        }
    }
}
