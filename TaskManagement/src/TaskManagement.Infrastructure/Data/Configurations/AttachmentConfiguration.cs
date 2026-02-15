using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Data.Configurations
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            builder.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            builder.Property(e => e.ContentType).HasMaxLength(100);
            builder.HasOne(e => e.Task).WithMany(t => t.Attachments).HasForeignKey(e => e.TaskId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
