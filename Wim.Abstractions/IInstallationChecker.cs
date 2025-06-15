namespace Wim.Abstractions
{
    public interface IInstallationChecker
    {
		/// <summary>
		/// Indicates whether the application is installed.
		/// </summary>
		public abstract bool IsInstalled { get; }

		/// <summary>
		/// Indicates whether the application is installed for all users (i.e., in the LocalMachine registry).
		/// </summary>
		public abstract bool IsRoot { get; }
    }
}
