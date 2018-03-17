using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Comments.Configuration
{
    public static class AppSettings 
    {
        // this is a static class for storing appsettings so we don't have to use DI for passing configuration stuff
        // (i.e. to stop us having to pass IOptions<SomeConfig> through the stack)

        public static EnvironmentConfig Environment { get; private set; }
        public static FeedConfig Feed { get; private set; }

        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EnvironmentConfig>(configuration.GetSection("AppSettings:Environment"));
            services.Configure<FeedConfig>(configuration.GetSection("Feeds")); //needs work

            var sp = services.BuildServiceProvider();
            Environment = sp.GetService<IOptions<EnvironmentConfig>>().Value;
            Feed = sp.GetService<IOptions<FeedConfig>>().Value;
        }
    }
}