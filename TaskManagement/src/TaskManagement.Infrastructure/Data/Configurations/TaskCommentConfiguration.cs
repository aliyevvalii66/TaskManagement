using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
    {
        public void Configure(EntityTypeBuilder<TaskComment> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Content).IsRequired().HasMaxLength(2000);
            builder.HasOne(e => e.Task).WithMany(t => t.Comments).HasForeignKey(e => e.TaskId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.User).WithMany(u => u.Comments).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
