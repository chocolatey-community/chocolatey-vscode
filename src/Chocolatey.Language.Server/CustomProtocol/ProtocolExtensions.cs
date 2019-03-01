using OmniSharp.Extensions.LanguageServer;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Chocolatey.Language.Server.CustomProtocol
{
    /// <summary>
    ///     Custom Language Server Protocol extensions.
    /// </summary>
    public static class ProtocolExtensions
    {
        /// <summary>
        ///     Update the configuration from the specified configuration-change notification.
        /// </summary>
        /// <param name="configuration">
        ///     The <see cref="Configuration"/> to update.
        /// </param>
        /// <param name="request">
        ///     The configuration-change notification.
        /// </param>
        public static void UpdateFrom(this Configuration configuration, DidChangeConfigurationObjectParams request)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            JObject json = request.Settings?.SelectToken(Configuration.SectionName) as JObject;

            if (json == null)
            {
                return;
            }

            configuration.UpdateFrom(json);
        }

        /// <summary>
        ///     Update the configuration from the specified initialisation request.
        /// </summary>
        /// <param name="configuration">
        ///     The <see cref="Configuration"/> to update.
        /// </param>
        /// <param name="request">
        ///     The initialisation request.
        /// </param>
        public static void UpdateFrom(this Configuration configuration, InitializeParams request)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            JToken initializationParameters = request.InitializationOptions as JToken;

            if (initializationParameters == null)
            {
                return;
            }

            JObject json = initializationParameters.SelectToken(Configuration.SectionName) as JObject;

            if (json == null)
            {
                return;
            }

            configuration.UpdateFrom(json);
        }

        /// <summary>
        ///     Update the configuration from the specified JSON.
        /// </summary>
        /// <param name="configuration">
        ///     The <see cref="Configuration"/> to update.
        /// </param>
        /// <param name="settingsJson">
        ///     A <see cref="JObject"/> representing the flattened settings JSON from VS Code.
        /// </param>
        public static void UpdateFrom(this Configuration configuration, JObject settingsJson)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (settingsJson == null)
            {
                throw new ArgumentNullException(nameof(settingsJson));
            }

            // Temporary workaround - JsonSerializer.Populate reuses existing HashSet.
            configuration.Language.SuppressedRules.Clear();

            using (JsonReader reader = settingsJson.CreateReader())
            {
                new JsonSerializer().Populate(reader, configuration);
            }
        }
    }
}
