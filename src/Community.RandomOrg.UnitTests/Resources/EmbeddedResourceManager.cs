using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Community.RandomOrg.UnitTests.Resources
{
    [DebuggerStepThrough]
    internal static class EmbeddedResourceManager
    {
        private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        private static readonly string _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        public static string GetString(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            using (var resourceStream = _assembly.GetManifestResourceStream(_assemblyName + "." + name))
            {
                if (resourceStream == null)
                {
                    throw new InvalidOperationException(FormattableString.Invariant($"The specified resource \"{name}\" is not found"));
                }

                using (var bufferStream = new MemoryStream((int)resourceStream.Length))
                {
                    resourceStream.CopyTo(bufferStream);

                    return Encoding.UTF8.GetString(bufferStream.ToArray());
                }
            }
        }
    }
}