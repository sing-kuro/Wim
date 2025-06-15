using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wim.Abstractions
{
	/// <summary>
	/// An enumeration representing the standard paths.
	/// </summary>
	public enum StdPath
	{
		/// <summary>
		/// The root path.
		/// </summary>
		Root,

		/// <summary>
		/// The path to the config directory.
		/// </summary>
		Config,

		/// <summary>
		/// The path to the data directory.
		/// </summary>
		Data,

		/// <summary>
		/// The path to the state directory.
		/// </summary>
		State,

		/// <summary>
		/// The path to the log directory.
		/// </summary>
		Log,

		/// <summary>
		/// The path to the plugins' data directory.
		/// </summary>
		PluginData,

		/// <summary>
		/// The path to the cache directory.
		/// </summary>
		Cache
	}

	/// <summary>
	/// An enumeration representing the root paths.
	/// </summary>
	/// <remarks>
	/// A root path is a path used to store data that is not user-specific.
	/// </remarks>
	public enum RootPath
	{
		/// <summary>
		/// The root path.
		/// </summary>
		Root,

		/// <summary>
		/// The path to the config directory.
		/// </summary>
		Config,

		/// <summary>
		/// The path to the data directory.
		/// </summary>
		Data,

		/// <summary>
		/// The path to the plugins' data directory.
		/// </summary>
		PluginData
	}

    public interface IRuntimePathManager
    {
		/// <summary>
		/// Gets the runtime paths.
		/// </summary>
		/// <returns>A list of runtime paths.</returns>
		public abstract Collection<string> GetRuntimePaths();

		/// <summary>
		/// Sets the runtime paths to a new list.
		/// </summary>
		/// <param name="newPaths">The new list of paths.</param>
		public abstract void SetRuntimePaths(Collection<string> paths);

		/// <summary>
		/// Removes a path from the runtime paths.
		/// </summary>
		/// <param name="path">The path to remove.</param>
		public abstract void RemovePath(string path);

		/// <summary>
		/// Retrieves a list of file paths for all plugin assemblies located in the runtime paths.
		/// </summary>
		/// <remarks>This method searches each directory in the predefined list of paths for files with a ".dll"
		/// extension. Only unique file paths are included in the returned list. The search is limited to the top-level
		/// directory and does not include subdirectories.</remarks>
		/// <returns>A list of strings containing the full file paths of all discovered plugin assemblies. If no plugin assemblies are
		/// found, the list will be empty.</returns>
		public abstract Collection<string> GetPluginPaths();

		/// <summary>
		/// Gets a list of all paths where a file with the specified name exists.
		/// </summary>
		/// <param name="fileName">The name of the file.</param>
		/// <returns>The full paths to the file.</returns>
		public abstract Collection<string> GetPaths(string fileName);

		/// <summary>
		/// Gets the standard path for the specified standard path enumeration.
		/// </summary>
		/// <param name="what">The standard path enumeration.</param>
		/// <returns>The full path for the specified standard path.</returns>
		public abstract string GetStdPath(StdPath what);

		/// <summary>
		/// Gets the root path for the specified root path enumeration.
		/// </summary>
		/// <param name="what">The root path enumeration.</param>
		/// <returns>The full path for the specified root path.</returns>
		/// <remarks>
		/// This method returns an empty string if the app is not installed for all users.
		public abstract string GetRootPath(RootPath what);
    }
}
