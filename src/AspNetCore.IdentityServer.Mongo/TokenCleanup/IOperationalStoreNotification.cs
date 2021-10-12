namespace AspNetCore.IdentityServer.Mongo.TokenCleanup
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface to model notifications from the TokenCleanup feature.
    /// </summary>
    public interface IOperationalStoreNotification
    {
        /// <summary>
        /// Notification for persisted grants being removed.
        /// </summary>
        /// <param name="persistedGrants"></param>
        /// <returns></returns>
        Task PersistedGrantsRemovedAsync(IEnumerable<string> persistedGrants, CancellationToken cancellationToken = default);

        /// <summary>
        /// Notification for device codes being removed.
        /// </summary>
        /// <param name="deviceCodes"></param>
        /// <returns></returns>
        Task DeviceCodesRemovedAsync(IEnumerable<string> deviceCodes, CancellationToken cancellationToken = default);
    }
}
