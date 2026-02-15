using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Project> Projects { get; }
        IRepository<ProjectMember> ProjectMembers { get; }
        IRepository<Domain.Entities.Task> Tasks { get; }
        IRepository<TaskComment> TaskComments { get; }
        IRepository<Attachment> Attachments { get; }
        IRepository<ActivityLog> ActivityLogs { get; }

        Task<int> SaveChangesAsync();
        Task<bool> BeginTransactionAsync();
        Task<bool> CommitTransactionAsync();
        Task<bool> RollbackTransactionAsync();
    }
}
