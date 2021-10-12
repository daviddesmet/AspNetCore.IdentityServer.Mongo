namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class IdentityResource
    {
        private string DebuggerDisplay => Name ?? $"{{{typeof(IdentityResource)}}}";

        [BsonId]
        [BsonRequired]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        public bool Enabled { get; set; } = true;

        [MaxLength(200)]
        public string? DisplayName { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool Required { get; set; }

        public bool Emphasize { get; set; }

        public bool ShowInDiscoveryDocument { get; set; } = true;

        public List<IdentityResourceClaim> UserClaims { get; set; } = new();

        public List<IdentityResourceProperty> Properties { get; set; } = new();

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime? Updated { get; set; }

        public bool NonEditable { get; set; }
    }
}
