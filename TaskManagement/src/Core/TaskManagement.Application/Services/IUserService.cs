using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Application.DTOs.User;
using TaskManagement.Shared.Results;

namespace TaskManagement.Application.Services
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetUserByIdAsync(Guid id);
        Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync();
        Task<Result<PaginatedResponseDto<UserDto>>> GetPagedUsersAsync(int pageNumber, int pageSize);
        Task<Result<UserDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Result> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
        Task<Result> DeleteUserAsync(Guid id);
    }
}
