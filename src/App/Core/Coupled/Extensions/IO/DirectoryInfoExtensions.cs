namespace ORBIT9000.Core.Extensions.IO;
public static class DirectoryInfoExtensions
{
    /// <summary>
    /// Retrieves all files matching the specified search pattern from the directory and its subdirectories,
    /// excluding files located in directories listed in the skipFolders array.
    /// </summary>
    /// <param name="directory">The root directory to search in.</param>
    /// <param name="searchPattern">The file search pattern (e.g., "*.dll").</param>
    /// <param name="skipFolders">An array of folder names to exclude from the search.</param>
    /// <returns>An enumerable collection of FileInfo objects representing the matching files.</returns>
    public static IEnumerable<FileInfo> GetFilesExcept(this DirectoryInfo directory, string searchPattern, string[] skipFolders)
    {
        // Stack to manage directories for processing
        Stack<DirectoryInfo> directoriesToProcess = new();
        directoriesToProcess.Push(directory);

        while (directoriesToProcess.Count > 0)
        {
            DirectoryInfo currentDirectory = directoriesToProcess.Pop();

            // Skip the directory if its name matches any in the skipFolders array
            if (skipFolders.Contains(currentDirectory.Name, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            // Attempt to retrieve files in the current directory
            FileInfo[] filesInCurrentDirectory;
            try
            {
                filesInCurrentDirectory = currentDirectory.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
            }
            catch
            {
                // Skip directories that cannot be accessed
                continue;
            }

            // Yield each file found in the current directory
            foreach (FileInfo file in filesInCurrentDirectory)
            {
                yield return file;
            }

            // Attempt to retrieve subdirectories for further processing
            DirectoryInfo[] subdirectories;
            try
            {
                subdirectories = currentDirectory.GetDirectories();
            }
            catch
            {
                // Skip subdirectories that cannot be accessed
                continue;
            }

            // Add subdirectories to the stack for processing
            foreach (DirectoryInfo subdirectory in subdirectories)
            {
                directoriesToProcess.Push(subdirectory);
            }
        }
    }
}