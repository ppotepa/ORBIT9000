using ORBIT9000.Core.Abstractions.Plugin;
using Serilog;

using SystemAssembly = System.Reflection.Assembly;

namespace ORBIT9000.Engine.Loaders.Assembly
{
    internal sealed class AssemblyLoader
    {
        private static readonly Serilog.ILogger _logger
            = Log.Logger.ForContext<AssemblyLoader>();

        private AssemblyLoader() { }

        public static AssemblyLoadResult TryLoadAssembly(FileInfo info, bool loadAsBinary = false)
        {
            bool containsPlugins = false;
            SystemAssembly? assembly = null;
            List<Exception> exceptions = new List<Exception>();
            Type[] pluginTypes = Array.Empty<Type>();

            try
            {
                if (loadAsBinary)
                {
                    byte[] bytes = File.ReadAllBytes(info.FullName);
                    assembly = SystemAssembly.Load(bytes);
                }
                else
                {
                    assembly = SystemAssembly.LoadFrom(info.FullName);
                }
                
                pluginTypes = assembly.GetTypes()
                    .Where(type => type.IsClass && typeof(IOrbitPlugin).IsAssignableFrom(type))
                    .ToArray();

                containsPlugins = pluginTypes.Any();

                if (!containsPlugins)
                {
                    _logger?.Warning("File does not contain any plugins. {FileName}", info.Name);
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger?.Error(ex, "File not found: {Path}", info.FullName);
                exceptions.Add(ex);
            }
            catch (BadImageFormatException ex)
            {
                _logger?.Error(ex, "Invalid assembly format: {Path}", info.FullName);
                exceptions.Add(ex);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Failed to load assembly from {Path}", info.FullName);
                exceptions.Add(ex);
            }

            return new AssemblyLoadResult(assembly, containsPlugins, pluginTypes, exceptions);
        }
    }
}
