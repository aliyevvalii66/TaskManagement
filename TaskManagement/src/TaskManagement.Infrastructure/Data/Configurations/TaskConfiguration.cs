
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskManagement.Domain.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<TaskManagement.Domain.Entities.Task> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Title).IsRequired().HasMaxLength(255);
            builder.Property(e => e.Description).HasMaxLength(2000);
            builder.HasOne(e => e.Project).WithMany(p => p.Tasks).HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.AssignedTo).WithMany(u => u.AssignedTasks).HasForeignKey(e => e.AssignedToId).OnDelete(DeleteBehavior.SetNull);
            builder.HasOne(e => e.ParentTask).WithMany(t => t.SubTasks).HasForeignKey(e => e.ParentTaskId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
