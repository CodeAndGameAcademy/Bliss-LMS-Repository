using LMS.Domain.Entities;
using LMS.Domain.Enums;
using LMS.Infrastructure.Data;
using LMS.StudentAPI.Constants;
using LMS.StudentAPI.DTOs.Auth;
using LMS.StudentAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.StudentAPI.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class StudentAuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public StudentAuthController(ApplicationDbContext context, IPasswordService passwordService, IJwtService jwtService)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        // Student Registration
        [HttpPost("student-register")]
        public async Task<IActionResult> StudentRegisteration(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(x => x.MobileNumber == request.MobileNumber))
                return BadRequest(new
                {
                    message = "Mobile number already exists"
                });

            if (await _context.Users.AnyAsync(x => x.Email == request.Email))
                return BadRequest(new
                {
                    message = "Email already exists"
                });

            if (await _context.Users.AnyAsync(x => x.PrimaryDeviceId == request.DeviceId))
                return BadRequest(new
                {
                    message = "Device already registered with other account"
                });
            else if (await _context.Users.AnyAsync(x => x.SecondaryDeviceId == request.DeviceId))
                return BadRequest(new
                {
                    message = "Device already registered with other account"
                });

            var user = new User
            {
                FullName = request.FullName,
                MobileNumber = request.MobileNumber,
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                Role = Role.STUDENT,
                Image = FileConstants.Defaults.UserImage,
                PrimaryDeviceId = request.DeviceId,
                PrimaryDeviceInfo = request.DeviceInfo
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "User registered successfully"
            });
        }

        // Login 
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            Console.WriteLine("Login API Called......");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.MobileNumber == request.MobileNumber && x.Role == Role.STUDENT && x.IsActive == true);

            if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
                return BadRequest(new { message = "Invalid credentials or Inactive" });

            // Check Device Used
            var isDeviceUsed = await _context.Users.AnyAsync(x => x.MobileNumber != request.MobileNumber &&
                          (x.PrimaryDeviceId == request.DeviceId || x.SecondaryDeviceId == request.DeviceId));

            if (isDeviceUsed)
            {
                return BadRequest(new { message = "Device already in use by another account." });
            }


            // Device Checking
            if (!string.IsNullOrEmpty(user.PrimaryDeviceId) && user.PrimaryDeviceId != request.DeviceId)
            {
                if (string.IsNullOrEmpty(user.SecondaryDeviceId))
                {
                    user.SecondaryDeviceId = request.DeviceId;
                    user.SecondaryDeviceInfo = request.DeviceInfo;

                    await _context.SaveChangesAsync();
                }
                else if (user.SecondaryDeviceId != request.DeviceId)
                {
                    return BadRequest(new { message = "Device limit exceeded. Only 2 devices allowed." });
                }
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            // Remove existing refresh token (if any)
            var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (existingToken != null)
            {
                _context.RefreshTokens.Remove(existingToken);
            }

            // Save refresh token
            _context.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                DeviceId = request.DeviceId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {                
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                FullName = user.FullName,
                Email = user.Email,
                Image = user.Image,                
            });
        }

        // Refresh Token
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request)
        {
            Console.WriteLine("Refresh API Called......");

            var token = await _context.RefreshTokens.Include(x => x.User).FirstOrDefaultAsync(x => x.Token == request.RefreshToken);

            if (token == null || token.ExpiresAt < DateTime.UtcNow)
                return Unauthorized(new { message = "Invalid refresh token" });

            if (token.DeviceId != request.DeviceId)
                return Unauthorized(new { message = "Invalid device" });

            // Remove old token (rotation)
            _context.RefreshTokens.Remove(token);

            var newAccessToken = _jwtService.GenerateAccessToken(token.User);
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Guid.NewGuid().ToString(),
                UserId = token.UserId,
                DeviceId = request.DeviceId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            return Ok(new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            });
        }

        // Logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(LogoutRequest request)
        {
            Console.WriteLine("Logout API Called......");

            try
            {
                // Extract userId EVEN from expired token
                var userIdClaim = User.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                    throw new Exception("");

                var userId = Guid.Parse(userIdClaim);

                // Delete refresh token
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.Token == request.RefreshToken);

                if (token != null)
                {
                    _context.RefreshTokens.Remove(token);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Logged out successfully" });
            }
            catch(Exception ex)
            {
                return Ok(new { message = "Logged out succcessfully"+ex.Message });
            }
            
        }
    }
}
