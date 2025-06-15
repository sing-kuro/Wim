using System.IO;
using System.Collections.ObjectModel;
using Wim.Abstractions;

namespace Wim
{
	/// <summary>
	/// A class that manages runtime paths.
	/// </summary>
    internal class RuntimePathManager : IRuntimePathManager
    {
		public RuntimePathManager(Constants constants, InstallationChecker installation, MessageManager message)
		{
			stdRoot = InitializeStdRoot(constants, installation);
			cache = InitializeCache(constants, installation);
			root = InitializeRoot(constants, installation);
			paths = InitializePaths(message);
        }

        /// <summary>
        /// Gets the runtime paths.
        /// </summary>
        /// <returns>A list of runtime paths.</returns>
        public Collection<string> GetRuntimePaths()
		{
			return paths;
		}

		/// <summary>
		/// Sets the runtime paths to a new list.
		/// </summary>
		/// <param name="newPaths">The new list of paths.</param>
		public void SetRuntimePaths(Collection<string> newPaths)
		{
			paths.Clear();
			if (newPaths != null)
			{
				foreach (var path in newPaths)
				{
					if (!paths.Contains(path))
					{
						paths.Add(path);
					}
				}
			}
		}

		/// <summary>
		/// Removes a path from the runtime paths.
		/// </summary>
		/// <param name="path">The path to remove.</param>
		public void RemovePath(string path)
		{
			paths.Remove(path);
		}

		/// <summary>
		/// Retrieves a list of file paths for all plugin assemblies located in the runtime paths.
		/// </summary>
		/// <remarks>This method searches each directory in the predefined list of paths for files with a ".dll"
		/// extension. Only unique file paths are included in the returned list. The search is limited to the top-level
		/// directory and does not include subdirectories.</remarks>
		/// <returns>A list of strings containing the full file paths of all discovered plugin assemblies. If no plugin assemblies are
		/// found, the list will be empty.</returns>
		public Collection<string> GetPluginPaths()
		{
            Collection<string> pluginPaths = [];
			foreach (var path in paths)
			{
				if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
				{
					continue;
                }
                DirectoryInfo dir = new(path);
				foreach (var p in dir.GetFiles("*.dll", SearchOption.TopDirectoryOnly))
				{
					if (!pluginPaths.Contains(p.FullName))
					{
						pluginPaths.Add(p.FullName);
					}
                }
            }
			return pluginPaths;
		}

		/// <summary>
		/// Gets a list of all paths where a file with the specified name exists.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <returns>The full paths to the file.</returns>
		public Collection<string> GetPaths(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				return [];
			}
			Collection<string> foundPaths = [];
			foreach (var path in paths)
			{
				var fullPath = Path.Combine(path, fileName);
				if (File.Exists(fullPath))
				{
					foundPaths.Add(fullPath);
				}
			}
			return foundPaths;
		}

		/// <summary>
		/// Gets the standard path for the specified standard path enumeration.
		/// </summary>
		/// <param name="what">The standard path enumeration.</param>
		/// <returns>The full path for the specified standard path.</returns>
		public string GetStdPath(StdPath what)
		{
            return what switch
            {
                StdPath.Root => stdRoot,
                StdPath.Config => Path.Combine(GetStdPath(StdPath.Root), "config"),
                StdPath.Data => Path.Combine(GetStdPath(StdPath.Root), "data"),
                StdPath.State => Path.Combine(GetStdPath(StdPath.Data), "state"),
                StdPath.Log => Path.Combine(GetStdPath(StdPath.Data), "log"),
                StdPath.PluginData => Path.Combine(GetStdPath(StdPath.Data), "plugins"),
                StdPath.Cache => cache,
                _ => string.Empty,
            };
        }

		/// <summary>
		/// Gets the root path for the specified root path enumeration.
		/// </summary>
		/// <param name="what">The root path enumeration.</param>
		/// <returns>The full path for the specified root path.</returns>
		/// <remarks>
		/// This method returns an empty string if the app is not installed for all users.
		public string GetRootPath(RootPath what)
		{
            switch (what)
            {
                case RootPath.Root:
                    return root;
                case RootPath.Config:
                {
                    var r = GetRootPath(RootPath.Root);
                    return string.IsNullOrEmpty(r) ? string.Empty : Path.Combine(r, "config");
                }
                case RootPath.Data:
                {
                    var r = GetRootPath(RootPath.Root);
                    return string.IsNullOrEmpty(r) ? string.Empty : Path.Combine(r, "data");
                }
                case RootPath.PluginData:
                {
                    var data = GetRootPath(RootPath.Data);
                    return string.IsNullOrEmpty(data) ? string.Empty : Path.Combine(data, "plugins");
                }
                default:
                    return string.Empty;
            }
		}

		/// <summary>
		/// Gets the path to the standard root directory.
		/// </summary>
		/// <returns>The path to the standard root directory.</returns>
        private static string InitializeStdRoot(Constants constants, InstallationChecker installation)
        {
            if (installation.IsInstalled)
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), constants.OrganizationName, constants.ApplicationName);
            }
            else
            {
                string? dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException("Unable to determine the directory of the executing assembly.");
                return Path.Combine(dir, "..");
            }
        }

		/// <summary>
		/// Gets the path to the cache directory.
		/// </summary>
		/// <returns>The path to the cache directory.</returns>
        private static string InitializeCache(Constants constants, InstallationChecker installation)
        {
            if (installation.IsInstalled)
            {
                return Path.Combine(Path.GetTempPath(), constants.OrganizationName, constants.ApplicationName);
            }
            else
            {
                string? dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException("Unable to determine the directory of the executing assembly.");
                return Path.Combine(dir, "..", "temp");
            }
        }

		/// <summary>
		/// Gets the root path.
		/// </summary>
		/// <returns>The root path.</returns>
		/// <remarks>
		/// This method returns an empty string if the app is not installed for all users.
		/// </remarks>
		private static string InitializeRoot(Constants constants, InstallationChecker installation)
		{
			if (installation.IsRoot)
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), constants.OrganizationName, constants.ApplicationName);
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the initial runtime paths.
		/// </summary>
		/// <returns>A list of initial runtime paths.</returns>
        private Collection<string> InitializePaths(MessageManager message)
        {
            Collection<string> initialPaths = new Collection<string>();
            string? directoryName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(directoryName))
            {
                message.NotifyAll("Error", "Unable to determine the directory of the executing assembly.");
            }
            else
            {
                initialPaths.Add(Path.Combine(directoryName, ".."));
            }
            foreach (var path in new List<string>
            {
                GetRootPath(RootPath.Config),
                GetRootPath(RootPath.Data),
                GetStdPath(StdPath.Config),
                GetStdPath(StdPath.Data),
            })
            {
                initialPaths.Add(path);
            }

            return initialPaths;
        }

    
		/// <summary>
		/// The standard root path.
		/// </summary>
		private readonly string stdRoot;

		/// <summary>
		/// The path to the cache directory.
		/// </summary>
		private readonly string cache;

		/// <summary>
		/// The root path.
		/// </summary>
		private readonly string root;

		/// <summary>
		/// The runtime paths.
		/// </summary>
		private readonly Collection<string> paths;
    }
}
