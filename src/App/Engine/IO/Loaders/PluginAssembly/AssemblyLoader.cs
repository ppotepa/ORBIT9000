using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly.Context;
using ORBIT9000.Engine.Loaders.Plugin.Results;
using System.Reflection;

namespace ORBIT9000.Engine.IO.Loaders.PluginAssembly
{
    internal sealed class AssemblyLoader : IAssemblyLoader
    {
        private static readonly Dictionary<string, PluginLoadContext> _loadContexts
            = new Dictionary<string, PluginLoadContext>();

        private readonly ILogger<AssemblyLoader> _logger;

        public AssemblyLoader(ILogger<AssemblyLoader> logger) 
        {
            this._logger = logger;
        }
       
        public TryLoadAssemblyResult TryLoadAssembly(FileInfo info, bool loadAsBinary = false)
        {
            loadAsBinary = true;
            Assembly? assembly = null;
            List<Exception> exceptions = new List<Exception>();

            Type[] pluginTypes = Array.Empty<Type>();

            try
            {
                PluginLoadContext loadContext = new PluginLoadContext(info.FullName);
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
                    .Where(type => type.IsClass && type.GetInterfaces().Contains(typeof(IOrbitPlugin)))
                    .ToArray();

            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, "File not found: {Path}", info.FullName);
                exceptions.Add(ex);
            }
            catch (BadImageFormatException ex)
            {
                _logger.LogError(ex, "Invalid assembly format: {Path}", info.FullName);
                exceptions.Add(ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load assembly from {Path}", info.FullName);
                exceptions.Add(ex);
            }

            return new TryLoadAssemblyResult(assembly, pluginTypes, exceptions);
        }

        public void UnloadAssembly(string assemblyPath)
        {
            if (_loadContexts.TryGetValue(assemblyPath, out var loadContext))
            {
                loadContext.Unload();
                _loadContexts.Remove(assemblyPath);
                _logger.LogInformation("Unloaded assembly: {Path}", assemblyPath);
            }
        }

        public void UnloadAssembly(FileInfo info) 
            => UnloadAssembly(info.FullName);

    }
}