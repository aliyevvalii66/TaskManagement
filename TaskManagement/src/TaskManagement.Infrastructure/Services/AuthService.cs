using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.DTOs.User;
using TaskManagement.Application.Repositories;
using TaskManagement.Application.Services;
using TaskManagement.Shared.Results;
using DomainUser = TaskManagement.Domain.Entities.User;

namespace TaskManagement.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email {Email}", loginDto.Email);

            if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
                return Result<LoginResponseDto>.Failure("Email and password are required");

            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email.ToLower());

            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found for email {Email}", loginDto.Email);
                return Result<LoginResponseDto>.Failure("Invalid email or password");
            }

            if (!VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed - invalid password for email {Email}", loginDto.Email);
                return Result<LoginResponseDto>.Failure("Invalid email or password");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed - user inactive for email {Email}", loginDto.Email);
                return Result<LoginResponseDto>.Failure("User account is inactive");
            }

            var token = GenerateJwtToken(user.Id, user.Email);
            var tokenExpiration = DateTime.UtcNow.AddHours(24);

            user.LastLoginAt = DateTime.UtcNow;
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var loginResponse = new LoginResponseDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = token,
                TokenExpiration = tokenExpiration
            };

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);
            return Result<LoginResponseDto>.Success(loginResponse, "Login successful");
        }

        public async Task<Result<LoginResponseDto>> RegisterAsync(CreateUserDto createUserDto)
        {
            _logger.LogInformation("Registration attempt for email {Email}", createUserDto.Email);

            if (string.IsNullOrWhiteSpace(createUserDto.FirstName))
                return Result<LoginResponseDto>.Failure("First Name is required");

            if (string.IsNullOrWhiteSpace(createUserDto.Email))
                return Result<LoginResponseDto>.Failure("Email is required");

            if (string.IsNullOrWhiteSpace(createUserDto.Password))
                return Result<LoginResponseDto>.Failure("Password is required");

            var existingUser = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == createUserDto.Email.ToLower());

            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed - email already exists {Email}", createUserDto.Email);
                return Result<LoginResponseDto>.Failure("Email already registered");
            }

            var user = new DomainUser
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email.ToLower(),
                PasswordHash = HashPassword(createUserDto.Password),
                PhoneNumber = createUserDto.PhoneNumber,
                Department = createUserDto.Department,
                Avatar = "default-avatar.png",
                CreatedBy = Guid.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var token = GenerateJwtToken(user.Id, user.Email);
            var tokenExpiration = DateTime.UtcNow.AddHours(24);

            var loginResponse = new LoginResponseDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = token,
                TokenExpiration = tokenExpiration
            };

            _logger.LogInformation("User {UserId} registered successfully", user.Id);
            return Result<LoginResponseDto>.Success(loginResponse, "Registration successful");
        }

        public string GenerateJwtToken(Guid userId, string email)
        {
            var jwtSecret = _configuration["JwtSettings:Secret"];
            var jwtIssuer = _configuration["JwtSettings:Issuer"];
            var jwtAudience = _configuration["JwtSettings:Audience"];
            var jwtExpirationHours = int.Parse(_configuration["JwtSettings:ExpirationHours"] ?? "24");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(jwtExpirationHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private bool VerifyPassword(string password, string hash)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedInput = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                var hashBytes = Convert.FromBase64String(hash);

                return hashedInput.SequenceEqual(hashBytes);
            }
        }
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
