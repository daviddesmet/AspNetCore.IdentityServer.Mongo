namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ApiScope
    {
        private string DebuggerDisplay => Name ?? $"{{{typeof(ApiScope)}}}";

        [BsonId]
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

        public List<ApiScopeClaim> UserClaims { get; set; } = new();

        public List<ApiScopeProperty> Properties { get; set; } = new();
    }
}
