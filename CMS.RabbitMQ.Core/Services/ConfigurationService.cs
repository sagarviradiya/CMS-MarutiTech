using System.IO;
using Microsoft.Extensions.Configuration;

namespace CMS.RabbitMQ.Core.Services
{
    internal static class ConfigurationService
    {
        private static readonly IConfiguration configuration;

        static ConfigurationService()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true);
            configuration = builder.Build();
        }

        public static string Get(string key)
        {
            return configuration.GetSection(key).Value;
        }
    }
}