namespace Wim.Abstractions
{
    public interface IPlugin
    {
        /// <summary>
        /// Gets the name of the author of the plugin.
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Gets the name of the plugin.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the version of the plugin.
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// Gets the description of the plugin.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Initializes the plugin with the main application instance.
        /// </summary>
        /// <param name="app">The main application instance.</param>
        public void Initialize(IApp app);

        /// <summary>
        /// Unloads the plugin, performing any necessary cleanup.
        /// </summary>
        public void Unload();
    }
}
