using System.Collections.Generic;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate the length of description in the package metadata.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/DescriptionWordCountMaximum4000Requirement.cs">Package validator maximum length requirement for description.</seealso>
    public class DescriptionMaximumWordCount : NuspecRuleBase
    {
        /// <summary>
        /// Gets the string Id for the rule, similar to choco0001
        /// </summary>
        public override string Id
        {
            get
            {
                return "choco0003";
            }
        }

        /// <summary>
        /// Gets the documentation Url for the rule
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return "https://gep13.github.io/chocolatey-vscode/docs/rules/choco0003";
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
            if (!package.Description.IsMissing)
            {
                var descriptionLength = package.Description.Value.Length;

                if (descriptionLength > 4000)
                {
                    yield return CreateDiagnostic(
                        package.Description,
                        "Description should not exceed 4000 characters.");
                }
            }
        }
    }
}
