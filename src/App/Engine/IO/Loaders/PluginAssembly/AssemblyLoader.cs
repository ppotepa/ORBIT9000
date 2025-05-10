using Microsoft.Extensions.Logging;
using ORBIT9000.Engine.IO.Loaders.PluginAssembly.Context;
using System.Reflection;

namespace ORBIT9000.Engine.IO.Loaders.PluginAssembly
{
    internal sealed class AssemblyLoader(ILogger<AssemblyLoader> logger) : IAssemblyLoader
    {
        private static readonly Dictionary<string, PluginLoadContext> _loadContexts
            = [];

        private readonly ILogger<AssemblyLoader> _logger = logger;

        public Assembly Load(FileInfo info, bool loadAsBinary = false)
        {
            try
            {
                PluginLoadContext loadContext = new(info.FullName);
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
                string contextualMessage = $"Failed to load assembly from {info.FullName}.";
                this._logger.LogError(ex, "Failed to load assembly from {A}", info.FullName);
                throw new InvalidOperationException(contextualMessage, ex);
            }
        }

        public void UnloadAssembly(string assemblyPath)
        {
            if (_loadContexts.TryGetValue(assemblyPath, out PluginLoadContext? loadContext))
            {
                loadContext.Unload();
                _loadContexts.Remove(assemblyPath);
                this._logger.LogDebug("Unloaded assembly: {Path}", assemblyPath);
            }
        }

        public void UnloadAssembly(FileInfo info)
            => this.UnloadAssembly(info.FullName);
    }
}