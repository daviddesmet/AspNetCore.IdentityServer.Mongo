namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ClientRedirectUri
    {
        private string DebuggerDisplay => RedirectUri ?? $"{{{typeof(ClientRedirectUri)}}}";

        [BsonRequired]
        [MaxLength(2000)]
        public string RedirectUri { get; set; } = default!;
    }
}
