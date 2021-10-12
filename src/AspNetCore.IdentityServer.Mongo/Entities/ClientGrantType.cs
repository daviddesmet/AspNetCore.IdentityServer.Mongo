namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ClientGrantType
    {
        private string DebuggerDisplay => GrantType ?? $"{{{typeof(ClientGrantType)}}}";

        [BsonRequired]
        [MaxLength(250)]
        public string GrantType { get; set; } = default!;
    }
}
