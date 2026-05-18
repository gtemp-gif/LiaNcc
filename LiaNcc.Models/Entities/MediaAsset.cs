using System;
using System.Collections.Generic;

namespace LiaNcc.Models.Entities
{
    public class MediaAsset
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public int FileSize { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<EntityMedia> EntityMedias { get; set; } = new List<EntityMedia>();
    }
}
