using Wim.Abstractions;
using Microsoft.Win32;

namespace Wim
{
    internal class InstallationChecker : IInstallationChecker
    {
		/// <summary>
		/// Indicates whether the application is installed.
		/// </summary>
		public bool IsInstalled { get; }

		/// <summary>
		/// Indicates whether the application is installed for all users (i.e., in the LocalMachine registry).
		/// </summary>
		public bool IsRoot{ get; }

		/// <summary>
		/// Static constructor to check the installation status of the application.
		/// It checks both the LocalMachine and CurrentUser registry keys for the application's subkey.
		/// If the subkey exists in LocalMachine, it is considered installed as root; if it exists in CurrentUser, it is considered installed for the current user.
		/// If neither key has data, it is considered not installed.
		/// </summary>
		public InstallationChecker()
		{
			string subKey = $@"Software\{Application.CompanyName}\{Application.ProductName}";
			if (RegistryKeyHasData(Registry.LocalMachine, subKey))
			{
				IsInstalled = true;
				IsRoot = true;
			}
			else if (RegistryKeyHasData(Registry.CurrentUser, subKey))
			{
				IsInstalled = true;
				IsRoot = false;
			}
			else
			{
				IsInstalled = false;
				IsRoot = false;
			}
		}

		/// <summary>
		/// Checks if the specified registry key has any data.
		/// </summary>
		/// <param name="baseKey">The base registry key (e.g., LocalMachine or CurrentUser).</param>
		/// <param name="subKey">The subkey to check.</param>
		/// <returns>True if the subkey exists and has data, otherwise false.</returns>
		private static bool RegistryKeyHasData(RegistryKey baseKey, string subKey)
		{
			using RegistryKey? key = baseKey.OpenSubKey(subKey);
			return key != null && (key.ValueCount > 0 || key.SubKeyCount > 0);
		}
    }
}
