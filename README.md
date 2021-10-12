# AspNetCore.IdentityServer.Mongo

[![CI](https://github.com/daviddesmet/AspNetCore.IdentityServer.Mongo/workflows/CI/badge.svg?branch=main)](https://github.com/daviddesmet/AspNetCore.IdentityServer.Mongo/actions)
[![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/daviddesmet/AspNetCore.IdentityServer.Mongo/issues)

## Introduction

MongoDB provider for Duende IdentityServer.

Supports the following stores in the [configuration](https://docs.duendesoftware.com/identityserver/v5/data/configuration/) data:

- [Client store](https://docs.duendesoftware.com/identityserver/v5/reference/stores/client_store/) for *Client* data.
- [CORS policy service](https://docs.duendesoftware.com/identityserver/v5/reference/stores/cors_policy_service/) for [CORS support](https://docs.duendesoftware.com/identityserver/v5/tokens/cors/).
- [Resource store](https://docs.duendesoftware.com/identityserver/v5/reference/stores/resource_store/) for *IdentityResource*, *ApiResource*, and *ApiScope* data.
- [Identity Provider store](https://docs.duendesoftware.com/identityserver/v5/reference/stores/idp_store/) for *IdentityProvider* data.

Supports the following stores in the [operational](https://docs.duendesoftware.com/identityserver/v5/data/operational/) data:

- [Grants](https://docs.duendesoftware.com/identityserver/v5/data/operational/grants/) for authorization and device codes, reference and refresh tokens, and remembered user consent.
- [Keys](https://docs.duendesoftware.com/identityserver/v5/data/operational/keys/) managing dynamically created signing keys.

## Installation

[![NuGet](https://buildstats.info/nuget/AspNetCore.IdentityServer.Mongo)](https://www.nuget.org/packages/AspNetCore.IdentityServer.Mongo/)

Install the AspNetCore.IdentityServer.Mongo NuGet package from the .NET Core CLI using:
```bash
dotnet add package AspNetCore.IdentityServer.Mongo
```

or from the NuGet package manager:
```bash
Install-Package AspNetCore.IdentityServer.Mongo
```

Or alternatively, you can add the AspNetCore.IdentityServer.Mongo package from within Visual Studio's NuGet package manager.

## Usage

[![](https://img.shields.io/nuget/dt/AspNetCore.IdentityServer.Mongo.svg)](https://www.nuget.org/packages/AspNetCore.IdentityServer.Mongo/)

### Configuration Store

For storing [configuration](https://docs.duendesoftware.com/identityserver/v5/data/configuration/) data, the configuration store can be used. This support provides implementations of the *IClientStore*, *IResourceStore*, *IIdentityProviderStore*, and the *ICorsPolicyService* extensibility points.

To use the configuration store support, use the *AddConfigurationStore* extension method after the call to *AddIdentityServer*:

```csharp
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    // Register MongoDB in the DI as usual, for example:
    services.AddSingleton<IMongoClient>(s =>
    {
        var mcs = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        return new MongoClient(mcs);
    });

    services.AddIdentityServer()
            // this adds the config data from DB (clients, resources, CORS)
            .AddConfigurationStore(options =>
            {
                options.DatabaseName = "<YOUR CONFIGURATION DATABASE NAME>";
            });
}
```

To configure the configuration store, use the *ConfigurationStoreOptions* options object passed to the configuration callback.

### Operational Store

For storing [operational](https://docs.duendesoftware.com/identityserver/v5/data/operational/) data, the operational store can be used. This support provides implementations of the *IPersistedGrantStore*, *IDeviceFlowStore*, and *ISigningKeyStore* extensibility points.

To use the operational store support, use the *AddOperationalStore* extension method after the call to *AddIdentityServer*:

```csharp
public IServiceProvider ConfigureServices(IServiceCollection services)
{
    // Register MongoDB in the DI as usual, for example:
    services.AddSingleton<IMongoClient>(s =>
    {
        var mcs = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        return new MongoClient(mcs);
    });

    services.AddIdentityServer()
            // this adds the operational data from DB (codes, tokens, consents)
            .AddOperationalStore(options =>
            {
                options.DatabaseName = "<YOUR OPERATIONAL DATABASE NAME>";

                // this enables automatic token cleanup. this is optional.
                options.EnableTokenCleanup = true;
                options.RemoveConsumedTokens = true;
                options.TokenCleanupInterval = 3600; // interval in seconds (default is 3600)
            });
}
```

To configure the operational store, use the *OperationalStoreOptions* options object passed to the configuration callback.

Collection names uses PascalCase but can be customized as needed using the corresponding configuration options.

### Samples

Contains samples for IdentityServer and IdentityServer with ASP.NET Identity integration.

## Learn More

- Duende IdentityServer [Data Stores and Persistence](https://docs.duendesoftware.com/identityserver/v5/data/).
- [Duende IdentityServer](https://duendesoftware.com/)
