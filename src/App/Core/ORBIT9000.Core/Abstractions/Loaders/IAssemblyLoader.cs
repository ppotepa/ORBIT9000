using ORBIT9000.Engine.Loaders.Plugin.Results;

namespace ORBIT9000.Core.Abstractions.Loaders
{
    public interface IAssemblyLoader
    {
        /// <summary>
        /// Attempts to load an Assembly from the specified file.
        /// </summary>
        /// <param name="info">The file information of the Assembly to load.</param>
        /// <param name="loadAsBinary">Indicates whether to load the Assembly as binary.</param>
        /// <returns>An <see cref="AssemblyLoadResult"/> containing the result of the load operation.</returns>
        TryLoadAssemblyResult TryLoadAssembly(FileInfo info, bool loadAsBinary = false);

        /// <summary>
        /// Unloads the Assembly at the specified path.
        /// </summary>
        /// <param name="assemblyPath">The path of the Assembly to unload.</param>
        void UnloadAssembly(string assemblyPath);
        void UnloadAssembly(FileInfo info);
    }
}
