using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace cx.API.Infrastructure.Core.Configurations
{
    public static class ConfigurationExtension
    {
        public static IConfigurationBuilder AddJsonConfigurationFiles(IConfigurationBuilder app, string environmentName)
        {
            if (app == null) return new ConfigurationBuilder();
            app.Sources.Clear();
            //Read Configuration from appSettings
            IConfigurationBuilder configBuilder = app.AddJsonFile($"appsettings.json", optional: true, reloadOnChange: true);
            if (!string.IsNullOrWhiteSpace(environmentName) && File.Exists($"appsettings.{environmentName}.json"))
            {
                configBuilder.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true);
            }

            return app;
        }
        public static IConfigurationRoot? ExpandEnvironmentVariables(this IConfigurationRoot configurtion, dynamic builder)
        {
            if (configurtion == null) return null;
            if (builder == null) return null;
            IEnumerable<IConfigurationProvider> providers = configurtion.Providers.Where(t => t.GetType() == typeof(JsonConfigurationProvider)).AsEnumerable();
            foreach (var item in providers)
            {
                PropertyInfo? propInfo = typeof(JsonConfigurationProvider).GetProperty("Data", BindingFlags.NonPublic |
                                                                            BindingFlags.Instance | BindingFlags.GetField);
                if (propInfo == null) continue;
                object? settingVariables = propInfo.GetValue(item);
                if (settingVariables == null) continue;
                foreach ((string key, string value) in ((Dictionary<string, string>)settingVariables).Where(t => t.Key != null && t.Value != null))
                {
                    string settingValue = value;
                    settingValue = Environment.ExpandEnvironmentVariables(settingValue);
                    builder.Configuration[key] = settingValue;
                }
            }

            return configurtion;
        }
    }
}
