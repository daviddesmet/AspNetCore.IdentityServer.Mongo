namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;
    using static Duende.IdentityServer.IdentityServerConstants;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Client
    {
        private string DebuggerDisplay => ClientId ?? $"{{{typeof(Client)}}}";

        [BsonId]
        [MaxLength(200)]
        public string ClientId { get; set; } = default!;

        public bool Enabled { get; set; } = true;

        [BsonRequired]
        [MaxLength(200)]
        public string ProtocolType { get; set; } = ProtocolTypes.OpenIdConnect;

        public List<ClientSecret> ClientSecrets { get; set; } = new();

        public bool RequireClientSecret { get; set; } = true;

        [MaxLength(200)]
        public string? ClientName { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? ClientUri { get; set; }

        [MaxLength(2000)]
        public string? LogoUri { get; set; }

        public bool RequireConsent { get; set; }

        public bool AllowRememberConsent { get; set; } = true;

        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        public List<ClientGrantType> AllowedGrantTypes { get; set; } = new();

        public bool RequirePkce { get; set; } = true;

        public bool AllowPlainTextPkce { get; set; }

        public bool RequireRequestObject { get; set; }

        public bool AllowAccessTokensViaBrowser { get; set; }

        public List<ClientRedirectUri> RedirectUris { get; set; } = new();

        public List<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; } = new();

        [MaxLength(2000)]
        public string? FrontChannelLogoutUri { get; set; }

        public bool FrontChannelLogoutSessionRequired { get; set; } = true;

        [MaxLength(2000)]
        public string? BackChannelLogoutUri { get; set; }

        public bool BackChannelLogoutSessionRequired { get; set; } = true;

        public bool AllowOfflineAccess { get; set; }

        public List<ClientScope> AllowedScopes { get; set; } = new();

        public int IdentityTokenLifetime { get; set; } = 300;

        [MaxLength(100)]
        public string? AllowedIdentityTokenSigningAlgorithms { get; set; }

        public int AccessTokenLifetime { get; set; } = 3600;

        public int AuthorizationCodeLifetime { get; set; } = 300;

        public int? ConsentLifetime { get; set; } = null;

        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;

        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;

        public int RefreshTokenUsage { get; set; } = (int)Duende.IdentityServer.Models.TokenUsage.OneTimeOnly;

        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

        public int RefreshTokenExpiration { get; set; } = (int)Duende.IdentityServer.Models.TokenExpiration.Absolute;

        public int AccessTokenType { get; set; } = (int)Duende.IdentityServer.Models.AccessTokenType.Jwt;

        public bool EnableLocalLogin { get; set; } = true;

        public List<ClientIdPRestriction> IdentityProviderRestrictions { get; set; } = new();

        public bool IncludeJwtId { get; set; }

        public List<ClientClaim> Claims { get; set; } = new();

        public bool AlwaysSendClientClaims { get; set; }

        [MaxLength(200)]
        public string ClientClaimsPrefix { get; set; } = "client_";

        [MaxLength(200)]
        public string? PairWiseSubjectSalt { get; set; }

        public List<ClientCorsOrigin> AllowedCorsOrigins { get; set; } = new();

        public List<ClientProperty> Properties { get; set; } = new();

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime? Updated { get; set; }

        public DateTime? LastAccessed { get; set; }

        public int? UserSsoLifetime { get; set; }

        [MaxLength(100)]
        public string? UserCodeType { get; set; }

        public int DeviceCodeLifetime { get; set; } = 300;

        public bool NonEditable { get; set; }
    }
}
