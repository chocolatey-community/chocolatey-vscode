using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate the length of description in the package metadata.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/DescriptionRequirement.cs">Package validator requirement for description.</seealso>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/DescriptionWordCountMaximum4000Requirement.cs">Package validator maximum length requirement for description.</seealso>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/DescriptionWordCountMinimum30Guideline.cs">Package validator minimum length guideline for description.</seealso>
    public class DescriptionLengthValidation : INuSpecRule
    {
        public IEnumerable<Diagnostic> Validate(XmlDocumentSyntax syntaxTree, TextPositions textPositions)
        {
            var descriptionElement = syntaxTree.DescendantNodes().OfType<XmlElementSyntax>().FirstOrDefault(x => string.Equals(x.Name, "description", StringComparison.OrdinalIgnoreCase));

            if (descriptionElement == null)
            {
                yield return new Diagnostic {
                    Message = "Description is required. See https://github.com/chocolatey/package-validator/wiki/DescriptionNotEmpty",
                    Severity = DiagnosticSeverity.Error,
                    Range = textPositions.GetRange(0, syntaxTree.End)
                };
                yield break;
            }

            var descriptionLength = descriptionElement.GetContentValue().Trim().Length;

            if (descriptionLength == 0)
            {
                var range = textPositions.GetRange(descriptionElement.StartTag.End, descriptionElement.EndTag.Start);

                yield return new Diagnostic {
                    Message = "Description is required. See https://github.com/chocolatey/package-validator/wiki/DescriptionNotEmpty",
                    Severity = DiagnosticSeverity.Error,
                    Range = range
                };
            }
            else if (descriptionLength <= 30)
            {
                var range = textPositions.GetRange(descriptionElement.StartTag.End, descriptionElement.EndTag.Start);

                yield return new Diagnostic {
                    Message = "Description should be sufficient to explain the software. See https://github.com/chocolatey/package-validator/wiki/DescriptionCharacterCountMinimum",
                    Severity = DiagnosticSeverity.Warning,
                    Range = range
                };
            }
            else if (descriptionLength > 4000)
            {
                var range = textPositions.GetRange(descriptionElement.StartTag.End, descriptionElement.EndTag.Start);

                yield return new Diagnostic {
                    Message = "Description should not exceed 4000 characters. See https://github.com/chocolatey/package-validator/wiki/DescriptionCharacterCountMaximum",
                    Severity = DiagnosticSeverity.Error,
                    Range = range
                };
            }
        }
    }
}
