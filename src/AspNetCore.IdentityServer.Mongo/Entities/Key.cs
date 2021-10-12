namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Models storage for keys.
    /// </summary>
    public class Key
    {
        [BsonId]
        public string Id { get; set; } = default!;

        public int Version { get; set; }

        public DateTime Created { get; set; }

        [BsonRequired]
        public string Use { get; set; } = default!;

        [BsonRequired]
        public string Algorithm { get; set; } = default!;

        public bool IsX509Certificate { get; set; }

        public bool DataProtected { get; set; }

        [BsonRequired]
        public string Data { get; set; } = default!;
    }
}
