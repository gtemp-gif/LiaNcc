using System;

namespace LiaNcc.Models.Entities
{
    public class EntityMedia
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string EntityName { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public Guid MediaAssetId { get; set; }
        public string MediaType { get; set; } = string.Empty;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public MediaAsset MediaAsset { get; set; } = null!;
    }
}
