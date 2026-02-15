using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Application.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data.Repositories;
using Task = TaskManagement.Domain.Entities.Task;

namespace TaskManagement.Infrastructure.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;
        private readonly ILogger<UnitOfWork> _logger;
        private readonly ILoggerFactory _loggerFactory;

        private IRepository<User> _users;
        private IRepository<Project> _projects;
        private IRepository<ProjectMember> _projectMembers;
        private IRepository<Task> _tasks;
        private IRepository<TaskComment> _taskComments;
        private IRepository<Attachment> _attachments;
        private IRepository<ActivityLog> _activityLogs;

        public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = logger;
            _loggerFactory = loggerFactory;
        }

        public IRepository<User> Users => _users ??= new Repository<User>(_context, _loggerFactory.CreateLogger<Repository<User>>());
        public IRepository<Project> Projects => _projects ??= new Repository<Project>(_context, _loggerFactory.CreateLogger<Repository<Project>>());
        public IRepository<ProjectMember> ProjectMembers => _projectMembers ??= new Repository<ProjectMember>(_context, _loggerFactory.CreateLogger<Repository<ProjectMember>>());
        public IRepository<Task> Tasks => _tasks ??= new Repository<Task>(_context, _loggerFactory.CreateLogger<Repository<Task>>());
        public IRepository<TaskComment> TaskComments => _taskComments ??= new Repository<TaskComment>(_context, _loggerFactory.CreateLogger<Repository<TaskComment>>());
        public IRepository<Attachment> Attachments => _attachments ??= new Repository<Attachment>(_context, _loggerFactory.CreateLogger<Repository<Attachment>>());
        public IRepository<ActivityLog> ActivityLogs => _activityLogs ??= new Repository<ActivityLog>(_context, _loggerFactory.CreateLogger<Repository<ActivityLog>>());

        public async Task<int> SaveChangesAsync()
        {
            _logger.LogInformation("Saving changes to database");
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> BeginTransactionAsync()
        {
            _logger.LogInformation("Beginning transaction");
            _transaction = await _context.Database.BeginTransactionAsync();
            return true;
        }

        public async Task<bool> CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();
                _logger.LogInformation("Transaction committed successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing transaction");
                await RollbackTransactionAsync();
                return false;
            }
            finally
            {
                await _transaction.DisposeAsync();
            }
        }

        public async Task<bool> RollbackTransactionAsync()
        {
            try
            {
                _logger.LogWarning("Rolling back transaction");
                await _transaction.RollbackAsync();
                return true;
            }
            finally
            {
                await _transaction.DisposeAsync();
            }
        }

        public void Dispose()
        {
            _context?.Dispose();
            _transaction?.Dispose();
        }
    }
}
