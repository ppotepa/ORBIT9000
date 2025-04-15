using ORBIT9000.Core.Plugin;
using System.Reflection;

namespace ORBIT9000.Engine.Loader
{
    internal class PluginLoader
    {
        private static (bool exists, bool isDll, bool containsPlugins, string[] errors, Assembly? assembly) TryLoadSingleFile(string path)
        {
            var errors = new string[] { };

            if (string.IsNullOrWhiteSpace(path))
            {
                return (false, false, false, ["Path is empty"], null);
            }

            if (File.Exists(path))
            {
                if (Path.GetExtension(path) != ".dll")
                {
                    return (false, false, false, ["File is not a DLL"], null);
                }
                try
                {
                    Assembly assembly = Assembly.LoadFile(path);
                    Type[] types = assembly.GetTypes();
                    IEnumerable<Type> pluginTypes = types.Where(t => t.IsClass && typeof(IOrbitPlugin).IsAssignableFrom(t));

                    if (!pluginTypes.Any())
                    {
                        return (true, true, false, ["No plugins found in the DLL"], null);
                    }
                    return (true, true, true, Array.Empty<string>(), assembly);
                }
                catch (Exception ex)
                {
                    return (true, false, false, [ex.Message], null);
                }
            }
            else
            {
                return (false, false, false, ["File does not exist"], null);
            }
        }

        public PluginLoadResult LoadSingle(string path)
        {
            var result = TryLoadSingleFile(path);
        }
    }
}
