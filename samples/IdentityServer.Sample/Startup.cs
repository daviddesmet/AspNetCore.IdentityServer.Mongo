namespace IdentityServer.Sample
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using MongoDB.Driver;

    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages()
                    .AddRazorRuntimeCompilation();

            var connectionString = Configuration.GetConnectionString("Mongo");

            services.AddSingleton<IMongoClient>(s =>
            {
                var mcs = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
                return new MongoClient(mcs);
            });

            services.AddIdentityServer()
                .AddTestUsers(TestUsers.Users)
                // this adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore(options => options.DatabaseName = Configuration.GetValue<string>("ConfigurationDatabaseName"))
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.DatabaseName = Configuration.GetValue<string>("OperationalDatabaseName");

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                    options.RemoveConsumedTokens = true;
                    options.TokenCleanupInterval = 10; // interval in seconds
                })
                // this is something you will want in production to reduce load on and requests to the DB
                .AddConfigurationStoreCache();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
