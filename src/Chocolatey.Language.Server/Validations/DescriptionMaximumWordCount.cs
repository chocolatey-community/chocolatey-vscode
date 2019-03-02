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
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/DescriptionWordCountMaximum4000Requirement.cs">Package validator maximum length requirement for description.</seealso>
    public class DescriptionMaximumWordCount : NuspecRuleBase
    {
        /// <summary>
        /// Gets the string Id for the rule, similar to CHOCO0001
        /// </summary>
        public override string Id
        {
            get
            {
                return "CHOCO0003";
            }
        }

        /// <summary>
        /// Gets the documentation Url for the rule
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return "https://gep13.github.io/chocolatey-vscode/docs/rules/CHOCO0003";
            }
        }

        public override IEnumerable<Diagnostic> Validate(XmlDocumentSyntax syntaxTree)
        {
            var descriptionElement = FindElementByName(syntaxTree, "description");

            if (descriptionElement != null)
            {
                var descriptionLength = descriptionElement.GetContentValue().Trim().Length;

                if (descriptionLength > 4000)
                {
                    yield return CreateRequirement(
                        descriptionElement,
                        "Description should not exceed 4000 characters.");
                }
            }
        }
    }
}
