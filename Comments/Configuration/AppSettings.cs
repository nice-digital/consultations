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
        public static GilliamConfig GilliamConfig { get; private set; }
		public static EncryptionConfig EncryptionConfig { get; private set; }
	    public static ReviewConfig ReviewConfig { get; set; }
	    public static StatusConfig StatusConfig { get; private set; }
	    public static ConsultationListConfig ConsultationListConfig { get; set; }

		public static void Configure(IServiceCollection services, IConfiguration configuration, string contentRootPath)
        {
            services.Configure<EnvironmentConfig>(configuration.GetSection("AppSettings:Environment"));
            services.Configure<FeedConfig>(configuration.GetSection("Feeds")); 
            services.Configure<GilliamConfig>(configuration.GetSection("Gilliam"));
	        services.Configure<EncryptionConfig>(configuration.GetSection("Encryption"));
	        services.Configure<ReviewConfig>(configuration.GetSection("Review"));
	        services.Configure<StatusConfig>(configuration.GetSection("Status"));
	        services.Configure<ConsultationListConfig>(configuration.GetSection("ConsultationList"));

			var sp = services.BuildServiceProvider();
            Environment = sp.GetService<IOptions<EnvironmentConfig>>().Value;
            Feed = sp.GetService<IOptions<FeedConfig>>().Value;
            GilliamConfig = sp.GetService<IOptions<GilliamConfig>>().Value;
            GilliamConfig.ContentRootPath = contentRootPath;
	        EncryptionConfig = sp.GetService<IOptions<EncryptionConfig>>().Value;
	        ReviewConfig = sp.GetService<IOptions<ReviewConfig>>().Value;
	        StatusConfig = sp.GetService<IOptions<StatusConfig>>().Value;
	        ConsultationListConfig = sp.GetService<IOptions<ConsultationListConfig>>().Value;
		}
    }
}
