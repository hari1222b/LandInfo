using Microsoft.AspNetCore.Mvc;
using LandInfoSystem.DTOs;
using LandInfoSystem.Services;

namespace LandInfoSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService, 
            IOtpService otpService, 
            IEmailService emailService,
            ISmsService smsService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _otpService = otpService;
            _emailService = emailService;
            _smsService = smsService;
            _logger = logger;
        }



        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<LoginResponseDto>> Register([FromBody] UserRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Email))
                return BadRequest("Username and Email are required");



            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Login user (Step 1 - Validate credentials and send OTP)
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                return BadRequest("Username and Password are required");

            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
                return Unauthorized(result);



            return Ok(result);
        }



        /// <summary>
        /// Logout user
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logout successful" });
        }
    }
}
