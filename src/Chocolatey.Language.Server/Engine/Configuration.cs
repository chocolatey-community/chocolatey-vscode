using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chocolatey.Language.Server.Engine
{
    public sealed class Configuration
    {
        /// <summary>
        ///     The name of the configuration section as passed in messages such as <see cref="CustomProtocol.DidChangeConfigurationObjectParams"/>.
        /// </summary>
        public static readonly string SectionName = "chocolatey";

        /// <summary>
        ///     The Chocolatey language service's main configuration.
        /// </summary>
        [JsonProperty("language", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public LanguageConfiguration Language { get; } = new LanguageConfiguration();

        /// <summary>
        ///     The Chocolatey Command Configuration
        /// </summary>
        [JsonProperty("commands", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public CommandsConfiguration Commands { get ; } = new CommandsConfiguration();


        [JsonProperty("templates", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public TemplatesConfiguration Templates { get; } = new TemplatesConfiguration();
    }

    public class TemplatesConfiguration
    {
        public TemplatesConfiguration()
        {
        }

        [JsonProperty("names", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public HashSet<string> Names { get; } = new HashSet<string>();

        [JsonProperty("source", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public string Source { get; } = "";
    }
    public class CommandsConfiguration
    {
        public CommandsConfiguration()
        {
        }

        [JsonProperty("new", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public NewCommandConfiguration NewCommand { get; } = new NewCommandConfiguration();
    }

    public class NewCommandConfiguration
    {
        public NewCommandConfiguration()
        {
        }

        [JsonProperty("properties", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public Dictionary<string, string> Properties { get; } = new Dictionary<string, string>();
    }

    /// <summary>
    ///     The main settings for the Chocolatey language service.
    /// </summary>
    public class LanguageConfiguration
    {
        /// <summary>
        ///     Create a new <see cref="LanguageConfiguration"/>.
        /// </summary>
        public LanguageConfiguration()
        {
        }

        /// <summary>
        ///     Allow suppressing of requirements
        /// </summary>
        [JsonProperty("allowSuppressionOfRequirements")]
        public bool AllowSuppressionOfRequirements { get; set; } = false;

        /// <summary>
        ///     Disable the language service?
        /// </summary>
        [JsonProperty("disableLanguageService")]
        public bool DisableLanguageService { get; set; } = false;

        /// <summary>
        ///     Types of object from the current project to include when offering completions.
        /// </summary>
        [JsonProperty("suppressedRules", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public HashSet<string> SuppressedRules { get; } = new HashSet<string>();
    }
}
