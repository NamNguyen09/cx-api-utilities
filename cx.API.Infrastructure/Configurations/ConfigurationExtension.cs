using System;

namespace cx.API.Infrastructure.Configurations
{
    public static class ConfigurationExtension
    {
        public static string ExpandEnvironmentVariable(this string variable)
        {
            if (string.IsNullOrEmpty(variable)) return "";

            return Environment.ExpandEnvironmentVariables(variable);
        }
    }
}