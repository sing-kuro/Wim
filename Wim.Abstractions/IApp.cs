namespace Wim.Abstractions
{
    public interface IApp
    {
        /// <summary>
        /// Loads a plugin into the application.
        /// </summary>
        /// <param name="pluginPath">The file path to the plugin assembly. Cannot be null or empty.</param>
		/// <returns><see langword="true"/> if the plugin was successfully loaded and registered; otherwise, <see langword="false"/>.</returns>
        public bool LoadPlugin(string pluginPath);

        /// <summary>
        /// Unloads the currently loaded plugin, releasing any associated resources.
        /// </summary>
		/// <param name="author">The author of the plugin to unload. Cannot be null or empty.</param>
        /// <param name="pluginName">The name of the plugin to unload. Cannot be null or empty.</param>
		/// <returns><see langword="true"/> if the plugin was successfully unloaded; otherwise, <see langword="false"/>.</returns>
        public bool UnloadPlugin(string author, string pluginName);

        /// <summary>
        /// Invokes a specified method on a plugin by name, passing optional parameters.
        /// </summary>
		/// <param name="author">The author of the plugin containing the method to invoke. Cannot be null or empty.</param>
        /// <param name="pluginName">The name of the plugin containing the method to invoke. Cannot be null or empty.</param>
        /// <param name="methodName">The name of the method to invoke on the plugin. Cannot be null or empty.</param>
		/// <param name="versionRange">The range of plugin versions to consider when invoking the method. This can be used to ensure compatibility with specific plugin versions.</param>
        /// <param name="parameters">An optional array of parameters to pass to the method. Can be null if no parameters are required.</param>
        /// <returns>The result of the invoked method, or <see langword="null"/> if the method does not return a value.</returns>
        public object? InvokePluginMethod(string author, string pluginName, string versionRange, string methodName, params object[]? parameters);

		/// <summary>
		/// Gets an instance of the Constants class, which provides access to application-wide constants.
		/// </summary>
		/// <returns>An instance of the Constants class.</returns>
		public IConstants Constants { get; }

		/// <summary>
		/// Gets an instance of the RuntimePathManager class, which provides access to runtime paths.
		/// </summary>
		/// <returns>An instance of the RuntimePathManager class.</returns>
		public IRuntimePathManager RuntimePathManager { get; }

		/// <summary>
		/// Gets an instance of the MessageManager class, which provides access to message handling functionality.
		/// </summary>
		/// <returns>An instance of the MessageManager class.</returns>
		public IMessageManager MessageManager { get; }

		/// <summary>
		/// Gets an instance of the InstallationChecker class, which provides access to installation checking functionality.
		/// </summary>
		/// <returns>An instance of the InstallationChecker class.</returns>
		public IInstallationChecker InstallationChecker { get; }

		/// <summary>
		/// Gets the main window of the application.
		/// </summary>
		/// <returns>The main window of the application</returns>
		public IMainWindow MainWindow { get; }
    }
}
