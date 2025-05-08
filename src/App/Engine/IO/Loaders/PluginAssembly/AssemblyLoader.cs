using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly.Context;
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

        public Assembly Load(FileInfo info, bool loadAsBinary = false)
        {
            try
            {
                PluginLoadContext loadContext = new PluginLoadContext(info.FullName);
                _loadContexts[info.FullName] = loadContext;

                if (loadAsBinary)
                {
                    byte[] bytes = File.ReadAllBytes(info.FullName);
                    return loadContext.LoadFromAssemblyBytes(bytes);
                }
                else
                {
                    return loadContext.LoadFromAssemblyPath(info.FullName);
                }
            }
            catch (Exception ex)
            {
                var contextualMessage = $"Failed to load assembly from {info.FullName}.";
                _logger.LogError(ex, contextualMessage);
                throw new InvalidOperationException(contextualMessage, ex);
            }
        }

        public void UnloadAssembly(string assemblyPath)
        {
            if (_loadContexts.TryGetValue(assemblyPath, out var loadContext))
            {
                loadContext.Unload();
                _loadContexts.Remove(assemblyPath);
                _logger.LogDebug("Unloaded assembly: {Path}", assemblyPath);
            }
        }

        public void UnloadAssembly(FileInfo info)
            => UnloadAssembly(info.FullName);
    }
}