using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagement.Application.Repositories;
using TaskManagement.Application.Services;
using TaskManagement.Application.Validators.User;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Data.Repositories;
using TaskManagement.Infrastructure.Data.UnitOfWork;
using TaskManagement.Infrastructure.Mappings;
using TaskManagement.Infrastructure.Services;

namespace TaskManagement.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            AddDatabaseServices(services, configuration);
            AddRepositoryServices(services);
            AddApplicationServices(services);
            AddValidationServices(services);
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            return services;
        }
        private static void AddValidationServices(IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(CreateUserValidator).Assembly);
        }
        private static void AddDatabaseServices(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        private static void AddRepositoryServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        private static void AddApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ITaskCommentService, TaskCommentService>();
            services.AddScoped<IProjectMemberService, ProjectMemberService>();
            services.AddScoped<IActivityLogService, ActivityLogService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
