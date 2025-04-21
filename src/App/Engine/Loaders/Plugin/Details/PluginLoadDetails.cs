using SystemAssembly = System.Reflection.Assembly;
namespace ORBIT9000.Engine.Loaders.Plugin.Details
{
    /// <summary>
    /// Contains all the relevant properties resulting from attempting to load a plugin file.
    /// Instead of returning multiple individual exceptions, <see cref="Error"/> provides a single summary message.
    /// </summary>
    internal record struct PluginLoadDetails(
        bool FileExists,
        bool IsDll,
        bool ContainsPlugins,
        string Error,
        SystemAssembly? LoadedAssembly,
        Type[] Plugins)
    {
        // Implicit conversion operators (if legacy code requires converting to/from tuple).
        public static implicit operator (bool FileExists, bool IsDll, bool ContainsPlugins, string? Error, SystemAssembly? LoadedAssembly, 
            Type[] plugins)(PluginLoadDetails value) 
            => (
            value.FileExists, value.IsDll, 
            value.ContainsPlugins,
            value.Error, 
            value.LoadedAssembly,
            value.Plugins
        );

        public static implicit operator PluginLoadDetails((bool FileExists, bool IsDll, bool ContainsPlugins, string Error, SystemAssembly? LoadedAssembly,
            Type[] Plugins) value) 
            => new PluginLoadDetails(
                value.FileExists, 
                value.IsDll, 
                value.ContainsPlugins, 
                value.Error, 
                value.LoadedAssembly,
                value.Plugins
            );
    }
}