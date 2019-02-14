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
    public class UrlValidValidation : INuSpecRule
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

        public IEnumerable<Diagnostic> Validate(XmlDocumentSyntax syntaxTree, TextPositions textPositions)
        {
            foreach (var elementName in UrlElements) {
                var element = syntaxTree.DescendantNodes().OfType<XmlElementSyntax>().FirstOrDefault(x => string.Equals(x.Name, elementName, StringComparison.OrdinalIgnoreCase));
                if (element != null) {
                    var uriString = element.GetContentValue().Trim();
                    Uri uri;
                    if(
                        !Uri.IsWellFormedUriString(uriString, UriKind.Absolute) ||
                        !Uri.TryCreate(uriString, UriKind.Absolute, out uri) ||
                        !uri.IsValid()
                    ) {
                        var range = textPositions.GetRange(element.StartTag.End, element.EndTag.Start);

                        yield return new Diagnostic {
                            Message = "Url in " + elementName + " is invalid. See https://github.com/chocolatey/package-validator/wiki/InvalidUrlProvided",
                            Severity = DiagnosticSeverity.Error,
                            Range = range
                        };
                    }
                }
            }
        }
    }
}
