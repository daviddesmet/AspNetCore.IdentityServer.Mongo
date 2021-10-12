namespace IdentityServer.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using AspNetCore.IdentityServer.Mongo.Entities;
    using AspNetCore.IdentityServer.Mongo.TokenCleanup;

    public class TestOperationalStoreNotification : IOperationalStoreNotification
    {
        public TestOperationalStoreNotification() => Console.WriteLine("ctor");

        public Task PersistedGrantsRemovedAsync(IEnumerable<string> persistedGrants, CancellationToken cancellationToken = default)
        {
            foreach (var grant in persistedGrants)
            {
                Console.WriteLine("cleaned: " + grant);
            }
            return Task.CompletedTask;
        }

        public Task DeviceCodesRemovedAsync(IEnumerable<string> deviceCodes, CancellationToken cancellationToken = default)
        {
            foreach (var deviceCode in deviceCodes)
            {
                Console.WriteLine("cleaned: " + deviceCode);
            }
            return Task.CompletedTask;
        }
    }
}
