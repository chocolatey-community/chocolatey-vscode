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
    public class DescriptionLengthValidation : NuspecRuleBase
    {
        public override IEnumerable<Diagnostic> Validate(XmlDocumentSyntax syntaxTree)
        {
            var descriptionElement = FindElementByName(syntaxTree, "description");

            if (descriptionElement == null)
            {
                yield return CreateDiagnostic(
                    0,
                    syntaxTree.End,
                    DiagnosticSeverity.Error,
                    "Description is required.",
                    "https://github.com/chocolatey/package-validator/wiki/DescriptionNotEmpty",
                    "DescriptionNotEmpty");
                yield break;
            }

            var descriptionLength = descriptionElement.GetContentValue().Trim().Length;

            if (descriptionLength == 0)
            {
                yield return CreateRequirement(
                    descriptionElement,
                    "Description is required.",
                    "https://github.com/chocolatey/package-validator/wiki/DescriptionNotEmpty",
                    "DescriptionNotEmpty");
            }
            else if (descriptionLength <= 30)
            {
                yield return CreateGuideline(
                    descriptionElement,
                    "Description should be sufficient to explain the software.",
                    "https://github.com/chocolatey/package-validator/wiki/DescriptionCharacterCountMinimum",
                    "DescriptionCharacterCountMinimum");
            }
            else if (descriptionLength > 4000)
            {
                yield return CreateRequirement(
                    descriptionElement,
                    "Description should not exceed 4000 characters.",
                    "https://github.com/chocolatey/package-validator/wiki/DescriptionCharacterCountMaximum",
                    "DescriptionCharacterCountMaximum");
            }
        }
    }
}
