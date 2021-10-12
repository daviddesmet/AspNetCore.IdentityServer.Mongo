namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Models storage for API resources.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ApiResource
    {
        private string DebuggerDisplay => Name ?? $"{{{typeof(ApiResource)}}}";

        [BsonId]
        [BsonRequired]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        public bool Enabled { get; set; } = true;

        [MaxLength(200)]
        public string? DisplayName { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? AllowedAccessTokenSigningAlgorithms { get; set; }

        public bool ShowInDiscoveryDocument { get; set; } = true;

        public bool RequireResourceIndicator { get; set; }

        public List<ApiResourceSecret> Secrets { get; set; } = new();

        public List<ApiResourceScope> Scopes { get; set; } = new();

        public List<ApiResourceClaim> UserClaims { get; set; } = new();

        public List<ApiResourceProperty> Properties { get; set; } = new();

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime? Updated { get; set; }

        public DateTime? LastAccessed { get; set; }

        public bool NonEditable { get; set; }
    }
}
