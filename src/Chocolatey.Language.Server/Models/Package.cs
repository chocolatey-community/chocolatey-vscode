using System.Collections.Generic;

namespace Chocolatey.Language.Server.Models
{
    /// <summary>
    /// Represents a single Chocolatey Package. This class cannot be inherited.
    /// </summary>
    public sealed class Package
    {
        private List<MetaValue<Dependency>> _dependencies = new List<MetaValue<Dependency>>();

        #region Package Related Properties

        /// <summary>
        /// Gets or sets the identifier of the chocolatey package.
        /// </summary>
        public MetaValue<string> Id { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets the version of the package.
        /// </summary>
        public MetaValue<string> Version { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets the package source URL.
        /// </summary>
        /// <remarks>Typically points to a publically facing source repository.</remarks>
        public MetaValue<string> PackageSourceUrl { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets a list of maintainers for the package.
        /// </summary>
        public IReadOnlyList<MetaValue<string>> Maintainers { get; set; } = new MetaValue<string>[0];

        #endregion Package Related Properties

        #region Software Related Properties

        /// <summary>
        /// Gets or sets the title of the software.
        /// </summary>
        public MetaValue<string> Title { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets a list authors for the software packaged.
        /// </summary>
        public IReadOnlyList<MetaValue<string>> Authors { get; set; } = new MetaValue<string>[0];

        /// <summary>
        /// Gets or sets the project URL for the packaged software.
        /// </summary>
        public MetaValue<string> ProjectUrl { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets the icon URL for the packaged software.
        /// </summary>
        public MetaValue<string> IconUrl { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets the copyright for the packaged software.
        /// </summary>
        public MetaValue<string> Copyright { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets the license URL for the packaged software.
        /// </summary>
        public MetaValue<string> LicenseUrl { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets wether the license needs to be accepted before it can be used.
        /// </summary>
        public MetaValue<bool> RequireLicenseAcceptance { get; set; } = new MetaValue<bool>();

        /// <summary>
        /// Gets or sets the project source URL for the packaged software.
        /// </summary>
        public MetaValue<string> ProjectSourceUrl { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets the documentation URL for the packaged software.
        /// </summary>
        public MetaValue<string> DocsUrl { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets the mailing list or forum URL for the packaged software.
        /// </summary>
        public MetaValue<string> MailingListUrl { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets the bug tracker URL for the packaged software.
        /// </summary>
        public MetaValue<string> BugTrackerUrl { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets the short summary of what the packaged software do.
        /// </summary>
        public MetaValue<string> Summary { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public IReadOnlyList<MetaValue<string>> Tags { get; set; } = new MetaValue<string>[0];

        /// <summary>
        /// Gets or sets the description of the packaged software.
        /// </summary>
        public MetaValue<string> Description { get; set; } = new MetaValue<string>();

        /// <summary>
        /// Gets or sets the release notes.
        /// </summary>
        public MetaValue<string> ReleaseNotes { get; set; } = new MetaValue<string>();

        #endregion Software Related Properties

        /// <summary>
        /// Gets the package dependencies.
        /// </summary>
        public IReadOnlyList<MetaValue<Dependency>> Dependencies => _dependencies.AsReadOnly();

        /// <summary>
        /// Gets the index of where the metadata element starts.
        /// </summary>
        public int StartsAt { get; internal set; }

        /// <summary>
        /// Gets the index of where the metadata opening element ends.
        /// </summary>
        public int EndsAt { get; internal set; }

        /// <summary>
        /// Gets or sets all elements specified in the nuspec xml that do not contain other xml elements.
        /// </summary>
        public IReadOnlyDictionary<string, MetaValue<string>> AllElements { get; set; }

        // Should files also be included?

        /// <summary>
        /// Adds a single dependency to the package dependency list.
        /// </summary>
        /// <param name="dependency">The dependency to add.</param>
        public void AddDependency(MetaValue<Dependency> dependency)
        {
            _dependencies.Add(dependency);
        }
    }
}
