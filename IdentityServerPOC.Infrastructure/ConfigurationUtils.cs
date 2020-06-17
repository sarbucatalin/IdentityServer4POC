using Microsoft.Extensions.Configuration;
using System.IO;

namespace IdentityServerPOC.Infrastructure
{
    public static class ConfigurationUtils
    {
        public static string GetConnectionString(string name)
        {
            var path = Directory.GetParent(Directory.GetCurrentDirectory()) + "\\IdentityServerPOC";

            var confBuilder = new ConfigurationBuilder().SetBasePath(path).AddJsonFile("appsettings.json");

            var configuration = confBuilder.Build();

            return configuration.GetConnectionString(name);
        }
    }
}
