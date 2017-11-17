using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Community.RandomOrg.Tests.Resources
{
    /// <summary>Represents a resource manager that provides convenient access to embedded resources at run time.</summary>
    [DebuggerStepThrough]
    internal static class EmbeddedResourceManager
    {
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private static readonly string _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>Returns the value of the specified embedded string resource.</summary>
        /// <param name="name">The name of the embedded resource to retrieve.</param>
        /// <returns>The value of the embedded resource.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" />.</exception>
        /// <exception cref="InvalidOperationException">The specified embedded resource is not found.</exception>
        public static string GetString(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (var bufferStream = new MemoryStream())
            {
                using (var resourceStream = _assembly.GetManifestResourceStream(FormattableString.Invariant($"{_assemblyName}.{name}")))
                {
                    if (resourceStream == null)
                    {
                        throw new InvalidOperationException(FormattableString.Invariant($"The specified resource \"{name}\" is not found"));
                    }

                    resourceStream.CopyTo(bufferStream);
                }

                return Encoding.UTF8.GetString(bufferStream.ToArray());
            }
        }
    }
}