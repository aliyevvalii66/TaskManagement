using System;
using System.Collections.Generic;
using System.Text;
using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Entities
{
    public class Attachment : BaseEntity
    {
        public Guid TaskId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Domain.Entities.Task Task { get; set; }
    }
}
