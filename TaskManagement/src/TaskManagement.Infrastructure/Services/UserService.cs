using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.User;
using TaskManagement.Application.Repositories;
using TaskManagement.Application.Services;
using TaskManagement.Domain.Entities;
using TaskManagement.Shared.Constants;
using TaskManagement.Shared.Results;
using DomainUser = TaskManagement.Domain.Entities.User;

namespace TaskManagement.Infrastructure.Services
{
    public class UserService : BaseService<User, UserDto>, IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger) : base(mapper, logger)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(Guid id)
        {
            _logger.LogInformation("Getting user with id {UserId}", id);
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found", id);
                return Result<UserDto>.Failure(AppConstants.ErrorMessages.UserNotFound);
            }

            var userDto = _mapper.Map<UserDto>(user);
            _logger.LogInformation("User with id {UserId} retrieved successfully", id);
            return Result<UserDto>.Success(userDto);
        }
        public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync()
        {
            _logger.LogInformation("Getting all users");
            var users = await _unitOfWork.Users.GetAllAsync();
            var userDtos = _mapper.Map<List<UserDto>>(users);

            _logger.LogInformation("Retrieved {Count} users", userDtos.Count);
            return Result<IEnumerable<UserDto>>.Success(userDtos);
        }
        public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            _logger.LogInformation("Creating new user with email {Email}", createUserDto.Email);

            if (string.IsNullOrWhiteSpace(createUserDto.FirstName))
                return Result<UserDto>.Failure(string.Format(AppConstants.ValidationMessages.FieldRequired, "First Name"));

            if (string.IsNullOrWhiteSpace(createUserDto.LastName))
                return Result<UserDto>.Failure(string.Format(AppConstants.ValidationMessages.FieldRequired, "Last Name"));

            if (string.IsNullOrWhiteSpace(createUserDto.Email))
                return Result<UserDto>.Failure(string.Format(AppConstants.ValidationMessages.FieldRequired, "Email"));

            if (string.IsNullOrWhiteSpace(createUserDto.Password))
                return Result<UserDto>.Failure(string.Format(AppConstants.ValidationMessages.FieldRequired, "Password"));

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

            var userDto = _mapper.Map<UserDto>(user);
            _logger.LogInformation("User created successfully with id {UserId} and email {Email}", user.Id, user.Email);
            return Result<UserDto>.Success(userDto, string.Format(AppConstants.SuccessMessages.RecordCreated, "User"));
        }
        public async Task<Result> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
        {
            _logger.LogInformation("Updating user with id {UserId}", id);
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found for update", id);
                return Result.Failure(AppConstants.ErrorMessages.UserNotFound);
            }

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.Department = updateUserDto.Department;
            user.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User with id {UserId} updated successfully", id);
            return Result.Success(string.Format(AppConstants.SuccessMessages.RecordUpdated, "User"));
        }
        public async Task<Result> DeleteUserAsync(Guid id)
        {
            _logger.LogInformation("Deleting user with id {UserId}", id);
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} not found for deletion", id);
                return Result.Failure(AppConstants.ErrorMessages.UserNotFound);
            }

            _unitOfWork.Users.Remove(user);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User with id {UserId} deleted successfully", id);
            return Result.Success(string.Format(AppConstants.SuccessMessages.RecordDeleted, "User"));
        }
        public async Task<Result<PaginatedResponseDto<UserDto>>> GetPagedUsersAsync(int pageNumber, int pageSize)
        {
            return await GetPagedAsync(
                (page, size) => _unitOfWork.Users.GetPagedAsync(page, size),
                pageNumber,
                pageSize,
                "Users"
            );
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
