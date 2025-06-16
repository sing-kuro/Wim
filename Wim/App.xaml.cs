using System.Windows;
using System.Reflection;
using Wim.Abstractions;

namespace Wim
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application, IApp
    {
		App()
		{
			Plugins = [];
			Constants = new Constants();
			InstallationChecker = new InstallationChecker();
			MessageManager = new MessageManager();
			RuntimePathManager = new RuntimePathManager((Constants)Constants, (InstallationChecker)InstallationChecker, (MessageManager)MessageManager);
		}

        /// <summary>
        /// Initializes the application and sets up the system tray icon.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			var trayIconName = ((Constants)Constants).TrayIconName;
			var paths = RuntimePathManager.GetPaths(trayIconName);
			Stream icon;
			if (paths.Count != 0)
			{
				icon = new FileStream(paths[0], FileMode.Open);
			}
			else
			{
				icon = GetResourceStream(new Uri(trayIconName, UriKind.Relative)).Stream;
			}
			var menu = new ContextMenuStrip();
			menu.Items.Add("Exit", null, Exit_Click);
			var notifyIcon = new NotifyIcon
			{
				Visible = true,
				Icon = new Icon(icon),
				Text = Constants.ApplicationName,
				ContextMenuStrip = menu
			};

			Current.MainWindow = new MainWindow();
			Current.MainWindow.Show();

			LoadPlugins();
		}

		/// <summary>
		/// Calls the shutdown method when the Exit menu item is clicked.
		/// </summary>
		private void Exit_Click(object? sender, EventArgs e)
		{
			Shutdown();
		}

		/// <summary>
		/// Handles the application exit event to perform any necessary cleanup.
		/// </summary>
		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

            CleanCache();
		}

		/// <summary>
		/// Cleans up the cache directory if it exists.
		/// </summary>
		private void CleanCache()
		{
			var cachePath = RuntimePathManager.GetStdPath(StdPath.Cache);
			if (Directory.Exists(cachePath))
			{
				DirectoryInfo cacheDir = new(cachePath);
				foreach (var file in cacheDir.GetFiles())
				{
					try
					{
						file.Delete();
					}
					catch
					{
						MessageManager.NotifyAll("Warning", $"Failed to delete file: {file.FullName}");
					}
				}
				foreach (var dir in cacheDir.GetDirectories())
				{
					try
					{
						dir.Delete(true);
					}
					catch
					{
						MessageManager.NotifyAll("Warning", $"Failed to delete directory: {dir.FullName}");
					}
				}
			}
		}


		/// <summary>
		/// Loads plugins from the paths provided by the runtime and initializes them.
		/// </summary>
		/// <remarks>This method attempts to load assemblies from the plugin paths retrieved via <see
		/// cref="RuntimePathManager.GetPluginPaths"/>. Each assembly is expected to contain a type named "Wim.Plugin" that
		/// can be instantiated. Successfully loaded plugins are added to the <see cref="Plugins"/> collection, keyed by their
		/// name. If a plugin fails to load, an error message is sent to all registered message handlers via <see
		/// cref="MessageManager.NotifyAll"/>.</remarks>
		private void LoadPlugins()
		{
			var pluginPaths = RuntimePathManager.GetPluginPaths();
			foreach (var pluginPath in pluginPaths)
			{
				LoadPlugin(pluginPath);
            }
		}

		/// <summary>
		/// Loads a plugin from the specified file path and registers it in the plugin collection.
		/// </summary>
		/// <remarks>The method attempts to load the specified assembly, locate the "Wim.Plugin" type, and create an
		/// instance of it. The plugin instance is then registered in the <c>Plugins</c> collection using its <c>Name</c>
		/// property as the key. If the <c>Name</c> property is not defined or cannot be retrieved, the plugin is registered
		/// with the key "Unknown Plugin".</remarks>
		/// <param name="pluginPath">The file path to the plugin assembly. This must be a valid path to a .NET assembly containing a type named
		/// "Wim.Plugin".</param>
		/// <exception cref="InvalidOperationException">Thrown if the plugin instance could not be created or is <see langword="null"/>.</exception>
		public void LoadPlugin(string pluginPath)
		{
			try
			{
				var assembly = Assembly.LoadFrom(pluginPath);
				var types = assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsAbstract && t.IsClass);
				foreach (var type in types)
				{
                    var pluginInstance = (IPlugin?)Activator.CreateInstance(type);
					if (pluginInstance == null)
					{
						MessageManager.NotifyAll("Error", $"Failed to create instance of plugin type {type.FullName} from {pluginPath}.");
						continue;
					}
					pluginInstance.Initialize(this);
					var pluginName = pluginInstance.Name;
					if (!Plugins.ContainsKey(pluginName))
					{
						Plugins[pluginName] = pluginInstance;
						MessageManager.NotifyAll("Info", $"Plugin '{pluginName}' loaded successfully.");
					}
					else
					{
						MessageManager.NotifyAll("Warning", $"Plugin '{pluginName}' is already loaded.");
                    }
                }
			}
			catch (Exception ex)
			{
				MessageManager.NotifyAll("Error", $"Failed to load plugin from {pluginPath}: {ex.Message}");
			}
        }

        /// <summary>
        /// Unloads the specified plugin by name, invoking its unload logic if available.
        /// </summary>
        /// <remarks>If the plugin defines an "Unload" method, it will be invoked before the plugin is removed. If the plugin is not found in the <c>Plugins</c> collection, a warning message is sent to all registered message handlers.</remarks>
        /// <param name="pluginName">The name of the plugin to unload. This must match the name of a currently loaded plugin.</param>
        public void UnloadPlugin(string pluginName)
		{
			if(Plugins.TryGetValue(pluginName, out var pluginInstance))
			{
				pluginInstance.GetType().GetMethod("Unload")?.Invoke(pluginInstance, [this]);
                Plugins.Remove(pluginName);
			}
			else
			{
				MessageManager.NotifyAll("Warning", $"Plugin '{pluginName}' not found.");
            }
        }

		public object? InvokePluginMethod(string pluginName, string methodName, params object[]? parameters)
		{
			if (Plugins.TryGetValue(pluginName, out var pluginInstance))
			{
            var methodKey = $"{pluginName}.{methodName}";
                var methodInfo = pluginInstance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                if (methodInfo == null)
                {
					MessageManager.NotifyAll("Error", $"Method '{methodName}' not found in plugin '{pluginName}'.");
                    return null;
                }
                var method = (Func<object[]?, object?>)Delegate.CreateDelegate(typeof(Func<object[], object?>), pluginInstance, methodInfo);
				return method(parameters);
			}
			else
			{
				MessageManager.NotifyAll("Error", $"Plugin '{pluginName}' not found.");
				return null;
            }
        }

        /// <summary>
        /// A dictionary of loaded plugins.
        /// </summary>
        private Dictionary<string, IPlugin> Plugins { get; }

		/// <summary>
		/// Provides access to various application constants and settings.
		/// </summary>
		public IConstants Constants { get; }

		/// <summary>
		/// Provides methods for checking the installation status of the application.
		/// </summary>
		public IInstallationChecker InstallationChecker { get; }

		/// <summary>
		/// Manages message transmission and subscription within the application.
		/// </summary>
		public IMessageManager MessageManager { get; }

		/// <summary>
		/// Manages runtime paths for the application, including configuration, data, and plugin directories.
		/// </summary>
		public IRuntimePathManager RuntimePathManager { get; }

		public new IMainWindow MainWindow => (IMainWindow)base.MainWindow;
    }
}
