using System;
using System.Collections.Generic;
using System.Linq;
using Chocolatey.Language.Server.Extensions;
using Chocolatey.Language.Server.Models;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate the length of description in the package metadata.
    /// </summary>
    /// TODO: Add <seealso> elements once PR is merged
    public class UrlSslCapable : NuspecRuleBase
    {
        private static readonly IReadOnlyCollection<string> UrlElements = new[]
        {
            "bugTrackerUrl",
            "docsUrl",
            "iconUrl",
            "licenseUrl",
            "mailingListUrl",
            "packageSourceUrl",
            "projectSourceUrl",
            "projectUrl",
            "wikiUrl",
        };

        /// <summary>
        /// Gets the string Id for the rule, similar to choco0001
        /// </summary>
        public override string Id
        {
            get
            {
                return "choco1002";
            }
        }

        /// <summary>
        /// Gets the documentation Url for the rule
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return "https://gep13.github.io/chocolatey-vscode/docs/rules/choco1002";
            }
        }

        /// <summary>
        /// Gets the type of of validation
        /// </summary>
        public override ValidationType ValidationType
        {
            get
            {
                return ValidationType.Guideline;
            }
        }

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            var elements = package.AllElements.Where(x => UrlElements.Any(u => string.Equals(x.Key, u, StringComparison.OrdinalIgnoreCase)));

            foreach (var element in elements)
            {
                var uriString = element.Value;
                if (
                    Uri.IsWellFormedUriString(uriString, UriKind.Absolute) &&
                    Uri.TryCreate(uriString, UriKind.Absolute, out Uri uri) &&
                    uri.IsValid()
                )
                {
                    if (uri.SslCapable())
                    {
                        yield return CreateDiagnostic(
                            element.Value,
                            $"Url in {element.Key} is SSL capable, please switch to https.");
                    }
                }
            }
        }
    }
}
