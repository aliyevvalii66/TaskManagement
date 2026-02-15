using AutoMapper;
using Microsoft.Extensions.Logging;
using TaskManagement.Application.DTOs.Common;
using TaskManagement.Shared.Results;

namespace TaskManagement.Infrastructure.Services
{
    public abstract class BaseService<TEntity, TDto>
    {
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        protected BaseService(IMapper mapper, ILogger logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        protected async Task<Result<PaginatedResponseDto<TDto>>> GetPagedAsync<T>(
            Func<int, int, Task<PaginatedResponseDto<T>>> getPagedFunc,
            int pageNumber,
            int pageSize,
            string entityName)
            where T : class
        {
            try
            {
                _logger.LogInformation("Getting paged {Entity} - Page: {PageNumber}, Size: {PageSize}",
                    entityName, pageNumber, pageSize);

                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var paged = await getPagedFunc(pageNumber, pageSize);
                var dtos = paged.Data.Select(d => _mapper.Map<TDto>(d)).ToList();

                var response = new PaginatedResponseDto<TDto>
                {
                    Data = dtos,
                    PageNumber = paged.PageNumber,
                    PageSize = paged.PageSize,
                    TotalCount = paged.TotalCount,
                    TotalPages = paged.TotalPages,
                    HasPreviousPage = paged.HasPreviousPage,
                    HasNextPage = paged.HasNextPage
                };

                return Result<PaginatedResponseDto<TDto>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paged {Entity}", entityName);
                return Result<PaginatedResponseDto<TDto>>.Failure(
                    $"Error retrieving {entityName}",
                    new List<string> { ex.Message });
            }
        }
    }
}
