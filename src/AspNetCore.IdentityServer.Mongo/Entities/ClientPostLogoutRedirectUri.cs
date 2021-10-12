namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ClientPostLogoutRedirectUri
    {
        private string DebuggerDisplay => PostLogoutRedirectUri ?? $"{{{typeof(ClientPostLogoutRedirectUri)}}}";

        [BsonRequired]
        [MaxLength(2000)]
        public string PostLogoutRedirectUri { get; set; } = default!;
    }
}
