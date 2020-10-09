using System;

namespace Comments.Configuration
{
    public class EnvironmentConfig
    {
        public string Name { get; set; }

	    public string AccountsEnvironment { get; set; } = "Live";

		public string HealthCheckEndpoint { get; set; }
    }
}
