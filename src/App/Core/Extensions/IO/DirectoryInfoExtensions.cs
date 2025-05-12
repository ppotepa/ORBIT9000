namespace ORBIT9000.Core.Extensions.IO
{
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Retrieves all files matching the specified search pattern from the directory and its subdirectories,
        /// excluding files located in directories listed in the skipFolders array.
        /// </summary>
        /// <param name="directory">The root directory to search in.</param>
        /// <param name="searchPattern">The file search pattern (e.g., "*.dll").</param>
        /// <param name="skipFolders">An array of folder names to exclude from the search.</param>
        /// <param name="skipFiles">An array of file name prefixes to exclude from the search.</param>
        /// <returns>An enumerable collection of FileInfo objects representing the matching files.</returns>
        public static IEnumerable<FileInfo> GetFilesExcept(this DirectoryInfo directory, string searchPattern, string[] skipFolders, string[] skipFiles)
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

                // Process files in the current directory
                foreach (FileInfo file in GetSafeDirectoryFiles(currentDirectory, searchPattern))
                {
                    if (!ShouldSkipFile(file, skipFiles))
                    {
                        yield return file;
                    }
                }

                // Process subdirectories
                foreach (DirectoryInfo subdirectory in GetSafeSubdirectories(currentDirectory))
                {
                    directoriesToProcess.Push(subdirectory);
                }
            }
        }

        private static FileInfo[] GetSafeDirectoryFiles(DirectoryInfo directory, string searchPattern)
        {
            try
            {
                return directory.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
            }
            catch
            {
                // Return empty array for inaccessible directories
                return Array.Empty<FileInfo>();
            }
        }

        private static DirectoryInfo[] GetSafeSubdirectories(DirectoryInfo directory)
        {
            try
            {
                return directory.GetDirectories();
            }
            catch
            {
                // Return empty array for inaccessible directories
                return Array.Empty<DirectoryInfo>();
            }
        }

        private static bool ShouldSkipFile(FileInfo file, string[] skipFilePrefixes)
        {
            return skipFilePrefixes.Any(prefix =>
                !string.IsNullOrEmpty(prefix) &&
                file.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}
