namespace AspNetIdentity.Sample
{
    using AspNetCore.Identity.Mongo;
    using AspNetCore.Identity.Mongo.Model;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using AspNetIdentity.Sample.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages()
                    .AddRazorRuntimeCompilation();

            var connectionString = Configuration.GetConnectionString("Mongo");

            services.AddIdentityMongoDbProvider<ApplicationUser, MongoRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 0;
            },
            mongo =>
            {
                mongo.ConnectionString = connectionString;
            }).AddDefaultTokenProviders();

            services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            services.AddIdentityServer()
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
                .AddConfigurationStoreCache()
                .AddAspNetIdentity<ApplicationUser>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

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