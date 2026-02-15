using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
    {
        public void Configure(EntityTypeBuilder<ProjectMember> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne(e => e.Project).WithMany(p => p.Members).HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(e => e.User).WithMany(u => u.ProjectMemberships).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            builder.HasIndex(e => new { e.ProjectId, e.UserId }).IsUnique();
        }
    }
}
