using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Engine.Loaders.Assembly.Context;

using SystemAssembly = System.Reflection.Assembly;

namespace ORBIT9000.Engine.Loaders.Assembly
{
    internal sealed class AssemblyLoader
    {
        // Collection to track load contexts so they're not garbage collected
        private static readonly Dictionary<string, PluginLoadContext> _loadContexts
            = new Dictionary<string, PluginLoadContext>();

        private static readonly ILogger _logger = LoggerFactory.Create(builder =>
        {            
            builder.SetMinimumLevel(LogLevel.Debug);
        })
        .CreateLogger<AssemblyLoader>();

        private AssemblyLoader() { }

        public static AssemblyLoadResult TryLoadAssembly(FileInfo info, bool loadAsBinary = false)
        {
            bool containsPlugins = false;

            SystemAssembly? assembly = null;
            List<Exception> exceptions = new List<Exception>();
            Type[] pluginTypes = Array.Empty<Type>();

            try
            {
                // Create custom load context for isolation
                var loadContext = new PluginLoadContext(info.FullName);
                _loadContexts[info.FullName] = loadContext;

                if (loadAsBinary)
                {
                    byte[] bytes = File.ReadAllBytes(info.FullName);
                    assembly = loadContext.LoadFromAssemblyBytes(bytes);
                }
                else
                {
                    assembly = loadContext.LoadFromAssemblyPath(info.FullName);
                }

                pluginTypes = assembly.GetTypes()
                    .Where(type => type.IsClass && typeof(IOrbitPlugin).IsAssignableFrom(type))
                    .ToArray();

                containsPlugins = pluginTypes.Any();

                if (!containsPlugins)
                {
                    _logger?.LogWarning("File does not contain any plugins. {FileName}", info.Name);
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger?.LogError(ex, "File not found: {Path}", info.FullName);
                exceptions.Add(ex);
            }
            catch (BadImageFormatException ex)
            {
                _logger?.LogError(ex, "Invalid assembly format: {Path}", info.FullName);
                exceptions.Add(ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to load assembly from {Path}", info.FullName);
                exceptions.Add(ex);
            }

            return new AssemblyLoadResult(assembly, containsPlugins, pluginTypes, exceptions);
        }

        public static void UnloadAssembly(string assemblyPath)
        {
            if (_loadContexts.TryGetValue(assemblyPath, out var loadContext))
            {
                loadContext.Unload();
                _loadContexts.Remove(assemblyPath);
                _logger?.LogInformation("Unloaded assembly: {Path}", assemblyPath);
            }
        }
    }
}