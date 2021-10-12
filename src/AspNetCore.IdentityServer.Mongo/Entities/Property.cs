namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public abstract class Property
    {
        private string DebuggerDisplay => Key ?? $"{{{typeof(Property)}}}";

        [BsonRequired]
        [MaxLength(250)]
        public string Key { get; set; } = default!;

        [BsonRequired]
        [MaxLength(2000)]
        public string Value { get; set; } = default!;
    }
}
