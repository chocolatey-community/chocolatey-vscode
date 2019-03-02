using System.Collections.Generic;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate the length of description in the package metadata.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/DescriptionRequirement.cs">Package validator requirement for description.</seealso>
    public class DescriptionRequiredValidation : NuspecRuleBase
    {
        /// <summary>
        /// Gets the string Id for the rule, similar to CHOCO0001
        /// </summary>
        public override string Id
        {
            get
            {
                return "CHOCO0002";
            }
        }

        /// <summary>
        /// Gets the documentation Url for the rule
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return "https://gep13.github.io/chocolatey-vscode/docs/rules/CHOCO0002";
            }
        }

        /// <summary>
        /// Gets the type of of validation
        /// </summary>
        public override ValidationType ValidationType
        {
            get
            {
                return ValidationType.Requirement;
            }
        }

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            if (package.Description.IsMissing)
            {
                yield return CreateDiagnostic(
                    package,
                    "Description is required.");
                yield break;
            }

            var descriptionLength = package.Description.Value.Trim().Length;

            if (descriptionLength == 0)
            {
                yield return CreateDiagnostic(
                    package.Description,
                    "Description is required.");
            }
        }
    }
}
