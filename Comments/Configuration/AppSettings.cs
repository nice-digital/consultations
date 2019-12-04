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
        public static FeedConfig Feed { get; set; }
		public static EncryptionConfig EncryptionConfig { get; private set; }
	    public static ReviewConfig ReviewConfig { get; set; }
	    public static StatusConfig StatusConfig { get; private set; }
	    public static ConsultationListConfig ConsultationListConfig { get; set; }

	    public static AuthenticationConfig AuthenticationConfig { get; set; }
	    public static GlobalNavConfig GlobalNavConfig { get; set; }

		public static void Configure(IServiceCollection services, IConfiguration configuration, string contentRootPath)
        {
            services.Configure<EnvironmentConfig>(configuration.GetSection("AppSettings:Environment"));
            services.Configure<FeedConfig>(configuration.GetSection("Feeds")); 
	        services.Configure<EncryptionConfig>(configuration.GetSection("Encryption"));
	        services.Configure<ReviewConfig>(configuration.GetSection("Review"));
	        services.Configure<StatusConfig>(configuration.GetSection("Status"));
	        services.Configure<ConsultationListConfig>(configuration.GetSection("ConsultationList"));
	        services.Configure<AuthenticationConfig>(configuration.GetSection("WebAppConfiguration"));
	        services.Configure<GlobalNavConfig>(configuration.GetSection("GlobalNav"));

			var sp = services.BuildServiceProvider();
            Environment = sp.GetService<IOptions<EnvironmentConfig>>().Value;
            Feed = sp.GetService<IOptions<FeedConfig>>().Value;
	        EncryptionConfig = sp.GetService<IOptions<EncryptionConfig>>().Value;
	        ReviewConfig = sp.GetService<IOptions<ReviewConfig>>().Value;
	        StatusConfig = sp.GetService<IOptions<StatusConfig>>().Value;
	        ConsultationListConfig = sp.GetService<IOptions<ConsultationListConfig>>().Value;
			AuthenticationConfig = AuthenticationConfig ?? sp.GetService<IOptions<AuthenticationConfig>>().Value; //null coalesce here to support tests
			GlobalNavConfig = sp.GetService<IOptions<GlobalNavConfig>>().Value;
		}
    }
}
