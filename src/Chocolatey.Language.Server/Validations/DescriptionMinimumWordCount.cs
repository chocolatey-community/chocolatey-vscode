using System;
using System.Collections.Generic;
using System.Linq;
using Chocolatey.Language.Server.Models;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate the length of description in the package metadata.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/DescriptionWordCountMinimum30Guideline.cs">Package validator minimum length guideline for description.</seealso>
    public class DescriptionMinimumWordCount : NuspecRuleBase
    {
        /// <summary>
        /// Gets the string Id for the rule, similar to CHOCO0001
        /// </summary>
        public override string Id
        {
            get
            {
                return "CHOCO1001";
            }
        }

        /// <summary>
        /// Gets the documentation Url for the rule
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return "https://gep13.github.io/chocolatey-vscode/docs/rules/CHOCO1001";
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
            if (!package.Description.IsMissing)
            {
                var descriptionLength = package.Description.Value.Trim().Length;

                if (descriptionLength <= 30)
                {
                    yield return CreateDiagnostic(
                        package.Description,
                        "Description should be sufficient to explain the software.");
                }
            }
        }
    }
}
