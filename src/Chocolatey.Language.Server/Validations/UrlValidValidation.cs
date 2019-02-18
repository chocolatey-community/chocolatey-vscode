using System;
using System.Collections.Generic;
using System.Linq;
using Chocolatey.Language.Server.Utility;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate the length of description in the package metadata.
    /// </summary>
    /// TODO: Add <seealso> elements once PR is merged
    public class UrlValidValidation : NuspecRuleBase
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

        public override IEnumerable<Diagnostic> Validate(XmlDocumentSyntax syntaxTree)
        {
            foreach (var elementName in UrlElements) {
                var element = syntaxTree.DescendantNodes().OfType<XmlElementSyntax>().FirstOrDefault(x => string.Equals(x.Name, elementName, StringComparison.OrdinalIgnoreCase));
                if (element != null) {
                    var uriString = element.GetContentValue().Trim();
                    if (
                        !Uri.IsWellFormedUriString(uriString, UriKind.Absolute) ||
                        !Uri.TryCreate(uriString, UriKind.Absolute, out Uri uri) ||
                        !uri.IsValid()
                    )
                    {
                        yield return CreateRequirement(
                            element,
                            $"Url in {elementName} is invalid.",
                            "https://github.com/chocolatey/package-validator/wiki/InvalidUrlProvided");
                    }
                }
            }
        }
    }
}
