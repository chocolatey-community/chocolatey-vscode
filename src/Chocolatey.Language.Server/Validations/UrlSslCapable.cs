using System;
using System.Collections.Generic;
using System.Linq;
using Chocolatey.Language.Server;
using Chocolatey.Language.Server.Extensions;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate the length of description in the package metadata.
    /// </summary>
    /// TODO: Add <seealso> elements once PR is merged
    public class UrlSslCapable : NuspecRuleBase
    {
        private static readonly IReadOnlyCollection<string> UrlElements = new []
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
        /// Gets the string Id for the rule, similar to CHOCO0001
        /// </summary>
        public override string Id
        {
            get
            {
                return "CHOCO1002";
            }
        }

        /// <summary>
        /// Gets the documentation Url for the rule
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return "https://gep13.github.io/chocolatey-vscode/docs/rules/CHOCO1002";
            }
        }

        public override IEnumerable<Diagnostic> Validate(XmlDocumentSyntax syntaxTree)
        {
            foreach (var elementName in UrlElements) {
                var element = FindElementByName(syntaxTree, elementName);
                if (element != null) {
                    var uriString = element.GetContentValue().Trim();
                    if (
                        Uri.IsWellFormedUriString(uriString, UriKind.Absolute) &&
                        Uri.TryCreate(uriString, UriKind.Absolute, out Uri uri) &&
                        uri.IsValid()
                    )
                    {
                        if (uri.SslCapable())
                        {
                            yield return CreateGuideline(
                                element,
                                "Url in " + elementName + " is SSL capable, please switch to https.");
                        }
                    }
                }
            }
        }
    }
}
