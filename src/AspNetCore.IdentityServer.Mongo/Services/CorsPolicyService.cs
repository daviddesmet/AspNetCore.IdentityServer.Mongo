namespace AspNetCore.IdentityServer.Mongo.Services
{
    using System;
    using System.Threading.Tasks;

    using Duende.IdentityServer.Services;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;

    using AspNetCore.IdentityServer.Mongo.Interfaces;

    /// <summary>
    /// Implementation of ICorsPolicyService that consults the client configuration in the database for allowed CORS origins.
    /// </summary>
    /// <seealso cref="ICorsPolicyService" />
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IConfigurationDbContext _context;
        private readonly ILogger<CorsPolicyService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorsPolicyService"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public CorsPolicyService(IConfigurationDbContext context, ILogger<CorsPolicyService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Determines whether origin is allowed.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            origin = origin.ToLowerInvariant();

            var count = await _context.ClientCorsOrigins.CountDocumentsAsync(x => x.Origin == origin);
            var isAllowed = count != 0;

            _logger.LogDebug("Origin {Origin} is allowed: {OriginAllowed}", origin, isAllowed);

            return isAllowed;
        }
    }
}
