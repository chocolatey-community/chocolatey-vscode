namespace Chocolatey.Language.Server.Models
{
    /// <summary>
    /// Represents a single package dependency. This class cannot be inherited.
    /// </summary>
    public sealed class Dependency
    {
        /// <summary>
        /// Gets or sets the identifier of the dependency.
        /// </summary>
        public MetaValue<string> Id { get; set; }

        /// <summary>
        /// Gets or sets the version range of the dependency.
        /// </summary>
        public MetaValue<string> VersionRange { get; set; }
    }
}
