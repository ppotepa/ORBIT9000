using SystemAssembly = System.Reflection.Assembly;
namespace ORBIT9000.Engine.IO.Loaders.Plugin.Details
{
    /// <summary>
    /// Contains all the relevant properties resulting from attempting to load a plugin file.
    /// Instead of returning multiple individual exceptions, <see cref="Error"/> provides a single summary message.
    /// </summary>
    //internal record struct AssemblyLoadDetails(
    //    bool FileExists,
    //    bool IsDll,
    //    bool ContainsPlugins,
    //    string Error,
    //    SystemAssembly? LoadedAssembly,
    //    Type[] Plugins)
    //{
    //    // Implicit conversion operators (if legacy code requires converting to/from tuple).
    //    public static implicit operator (bool FileExists, bool IsDll, bool ContainsPlugins, string? Error, SystemAssembly? LoadedAssembly, 
    //        Type[] Plugins)(AssemblyLoadDetails value) 
    //        => (
    //        value.FileExists, value.IsDll, 
    //        value.ContainsPlugins,
    //        value.Error, 
    //        value.LoadedAssembly,
    //        value.Plugins
    //    );

    //    public static implicit operator AssemblyLoadDetails((bool FileExists, bool IsDll, bool ContainsPlugins, string Error, SystemAssembly? LoadedAssembly,
    //        Type[] Plugins) value) 
    //        => new AssemblyLoadDetails(
    //            value.FileExists, 
    //            value.IsDll, 
    //            value.ContainsPlugins, 
    //            value.Error, 
    //            value.LoadedAssembly,
    //            value.Plugins
    //        );
    //}
}