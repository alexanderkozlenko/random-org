// © Alexander Kozlenko. Licensed under the MIT License.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace Anemonis.Runtime.Configuration
{
    [DebuggerStepThrough]
    internal sealed class AssemblyConfiguration
    {
        private readonly IConfiguration _configuration;

        private AssemblyConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static AssemblyConfiguration Load(Assembly assembly, string configuration)
        {
            if (assembly is null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var configurationFilePath = Path.Combine(Path.GetDirectoryName(assembly.Location), $"{assembly.GetName().Name}.{configuration}.json");
            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddJsonFile(configurationFilePath, true, false);

            return new(configurationBuilder.Build());
        }

        public string GetString(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return _configuration[name] ?? string.Empty;
        }
    }
}
