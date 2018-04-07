using NICE.Auth.NetCore.Configuration;
using System;

namespace NICE.Auth.NETCore.WebTest.Configuration
{
    public class GilliamConfig : IGilliamConfig
    {
        public string GilliamBasePath { get; set; }
        public Uri GilliamBasePathUri => new Uri(GilliamBasePath);

        public string GetClaimsUrl { get; set; }
        public string Realm { get; set; }
        public string ContentRootPath { get; set; }

        public int MaxClockSkewSeconds { get; set; }
        public TimeSpan MaxClockSkew => TimeSpan.FromSeconds(MaxClockSkewSeconds);
        public int CookieDurationMinutes { get; set; }
    }
}
