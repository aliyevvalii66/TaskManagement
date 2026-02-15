using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.Auth;
using TaskManagement.Application.DTOs.User;
using TaskManagement.Shared.Results;

namespace TaskManagement.Application.Services
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginDto loginDto);
        Task<Result<LoginResponseDto>> RegisterAsync(CreateUserDto createUserDto);
        string GenerateJwtToken(Guid userId, string email);
    }
}
