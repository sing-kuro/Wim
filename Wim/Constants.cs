using Wim.Abstractions;

namespace Wim
{
    internal class Constants : IConstants
    {
        /// <summary>
        /// The name of the application.
        /// </summary>
        public string ApplicationName { get; } = Application.ProductName ?? "Wim";

        /// <summary>
        /// The name of the organization.
        /// </summary>
        public string OrganizationName { get; } = Application.CompanyName ?? "Kuro Amami";

        /// <summary>
        /// The version of the application.
        /// </summary>
        public string ApplicationVersion { get; } = Application.ProductVersion;

        /// <summary>
        /// The name of the tray icon file.
        /// </summary>
        public string TrayIconName { get; } = "Wim.ico";
    }
}
