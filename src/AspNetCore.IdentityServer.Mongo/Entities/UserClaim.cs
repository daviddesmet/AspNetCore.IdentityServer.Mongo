namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public abstract class UserClaim
    {
        private string DebuggerDisplay => Type ?? $"{{{typeof(UserClaim)}}}";

        [BsonRequired]
        [MaxLength(200)]
        public string Type { get; set; } = default!;
    }
}
