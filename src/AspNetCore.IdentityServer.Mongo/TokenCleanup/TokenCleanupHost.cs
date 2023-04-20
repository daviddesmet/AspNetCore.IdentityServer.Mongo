namespace AspNetCore.IdentityServer.Mongo.TokenCleanup
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;

    using AspNetCore.IdentityServer.Mongo.Configuration;
    using AspNetCore.IdentityServer.Mongo.Entities.Jobs;
    using AspNetCore.IdentityServer.Mongo.Interfaces;

    /// <summary>
    /// Service to cleanup expired persisted grants.
    /// </summary>
    public class TokenCleanupHost : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly OperationalStoreOptions _options;
        private readonly ILogger<TokenCleanupHost> _logger;

        private TimeSpan CleanupInterval => TimeSpan.FromSeconds(_options.TokenCleanupInterval);

        private CancellationTokenSource? _source;

        /// <summary>
        /// Constructor for TokenCleanupHost.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public TokenCleanupHost(IServiceProvider serviceProvider, OperationalStoreOptions options, ILogger<TokenCleanupHost> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Starts the token cleanup polling.
        /// </summary>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.EnableTokenCleanup)
            {
                if (_source != null)
                    throw new InvalidOperationException("Already started. Call Stop first.");

                _logger.LogDebug("Starting grant removal");

                _source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                Task.Factory.StartNew(() => StartInternalAsync(_source.Token));
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the token cleanup polling.
        /// </summary>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_options.EnableTokenCleanup)
            {
                if (_source is null)
                    throw new InvalidOperationException("Not started. Call Start first.");

                _logger.LogDebug("Stopping grant removal");

                _source.Cancel();
                _source = null;
            }

            return Task.CompletedTask;
        }

        private async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested. Exiting.");
                    break;
                }

                try
                {
                    await Task.Delay(CleanupInterval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug("TaskCanceledException. Exiting.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Task.Delay exception: {ex.Message}. Exiting.");
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested. Exiting.");
                    break;
                }

                await CreateJobDbEntriesAsync(cancellationToken);
                await RemoveExpiredGrantsAsync(cancellationToken);
            }
        }

        private async Task CreateJobDbEntriesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
                var db = serviceScope.ServiceProvider.GetRequiredService<IJobSchedulerDbContext>();

                FilterDefinition<JobSchedule> Filter(string taskId) => Builders<JobSchedule>.Filter.Eq(x => x.TaskId, taskId);
                UpdateDefinition<JobSchedule> Update(string taskId) => Builders<JobSchedule>.Update.Set(x => x.TaskId, taskId);
                var options = new UpdateOptions { IsUpsert = true };

                if (await db.Jobs.CountDocumentsAsync(Filter(TokenCleanupService.REMOVE_EXPIRED_PERSISTEDGRANTS_TASKID)) == 0)
                    await db.Jobs.UpdateOneAsync(Filter(TokenCleanupService.REMOVE_EXPIRED_PERSISTEDGRANTS_TASKID), Update(TokenCleanupService.REMOVE_EXPIRED_PERSISTEDGRANTS_TASKID), options, cancellationToken);

                if (await db.Jobs.CountDocumentsAsync(Filter(TokenCleanupService.REMOVE_CONSUMED_PERSISTEDGRANTS_TASKID)) == 0)
                    await db.Jobs.UpdateOneAsync(Filter(TokenCleanupService.REMOVE_CONSUMED_PERSISTEDGRANTS_TASKID), Update(TokenCleanupService.REMOVE_CONSUMED_PERSISTEDGRANTS_TASKID), options, cancellationToken);

                if (await db.Jobs.CountDocumentsAsync(Filter(TokenCleanupService.REMOVE_DEVICECODES_TASKID)) == 0)
                    await db.Jobs.UpdateOneAsync(Filter(TokenCleanupService.REMOVE_DEVICECODES_TASKID), Update(TokenCleanupService.REMOVE_DEVICECODES_TASKID), options, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception creating Job Scheduler persistence: {ex.Message}");
            }
        }

        private async Task RemoveExpiredGrantsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
                var tokenCleanupService = serviceScope.ServiceProvider.GetRequiredService<TokenCleanupService>();
                await tokenCleanupService.RemoveExpiredGrantsAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception removing expired grants: {ex.Message}");
            }
        }
    }
}
