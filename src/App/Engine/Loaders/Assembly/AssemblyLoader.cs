using ORBIT9000.Core.Abstractions.Plugin;
using Serilog;
using SystemAssembly = System.Reflection.Assembly;

namespace ORBIT9000.Engine.Loaders.Assembly
{
    internal class AssemblyLoader
    {
        private static readonly Serilog.ILogger _logger = Log.Logger.ForContext<AssemblyLoader>();

        public static (SystemAssembly Assembly, bool ContainsPlugins, Type[] plugins) TryLoadAssembly(string path, List<Exception> exceptions)
        {
            var info = new FileInfo(path);
            SystemAssembly? assembly = null;
            bool containsPlugins = false;
            Type[] pluginTypes = Array.Empty<Type>();

            try
            {
                byte[] assemblyBytes = File.ReadAllBytes(path);
                assembly = SystemAssembly.Load(assemblyBytes);
                pluginTypes = assembly.GetTypes()
                    .Where(type => type.IsClass && typeof(IOrbitPlugin).IsAssignableFrom(type))
                    .ToArray();

                containsPlugins = pluginTypes.Any();

                if (!containsPlugins)
                {
                    _logger?.Warning("File does not contain any plugins. {FileName}", info.Name);
                }
            }
            catch (Exception ex)
            {
                _logger?.Warning(ex, "Failed to load assembly from {Path}", path);
                exceptions.Add(ex);
            }

            return (assembly, containsPlugins, pluginTypes);
        }
    }
}
