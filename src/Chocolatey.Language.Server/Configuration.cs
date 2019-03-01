using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Chocolatey.Language.Server
{
    public sealed class Configuration
    {
        /// <summary>
        ///     The name of the configuration section as passed in messages such as <see cref="CustomProtocol.DidChangeConfigurationObjectParams"/>.
        /// </summary>
        public static readonly string SectionName = "chocoLanguageServer";

        /// <summary>
        ///     The MSBuild language service's main configuration.
        /// </summary>
        [JsonProperty("language", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public LanguageConfiguration Language { get; } = new LanguageConfiguration();
    }

    /// <summary>
    ///     The main settings for the MSBuild language service.
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
        ///     Disable the language service?
        /// </summary>
        [JsonProperty("useClassicProvider")]
        public bool DisableLanguageService { get; set; } = false;

        /// <summary>
        ///     Types of object from the current project to include when offering completions.
        /// </summary>
        [JsonProperty("suppressedRules", ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public HashSet<string> SuppressedRules { get; } = new HashSet<string>();
    }
}
