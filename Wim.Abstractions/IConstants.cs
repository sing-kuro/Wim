namespace Wim.Abstractions
{
    public interface IConstants
    {
        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        public abstract string ApplicationName { get; }

        /// <summary>
        /// Gets the name of the organization.
        /// </summary>
        public abstract string OrganizationName { get; }

        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        public abstract string ApplicationVersion { get; }
    }
}
