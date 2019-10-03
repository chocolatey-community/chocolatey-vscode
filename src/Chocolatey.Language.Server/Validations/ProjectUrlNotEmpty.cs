using System.Collections.Generic;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    /// Handler to validate the project url is not missing and is not empty.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/PackageIdDoesNotEndWithConfigRequirement.cs">
    /// Package validator rule for project url not being empty.
    /// </seealso>
    public sealed class ProjectUrlNotEmpty : NuspecRuleBase
    {
        private const string VALIDATION_MESSAGE = "ProjectUrl (projectUrl) in the nuspec file is required. Please add it to the nuspec.";

        public override string Id => "choco0011";

        public override ValidationType ValidationType => ValidationType.Requirement;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            if (package.ProjectUrl.IsMissing)
            {
                yield return CreateDiagnostic(package, VALIDATION_MESSAGE);
            }
            else if (string.IsNullOrWhiteSpace(package.ProjectUrl))
            {
                yield return CreateDiagnostic(package.ProjectUrl, VALIDATION_MESSAGE);
            }
        }
    }
}
