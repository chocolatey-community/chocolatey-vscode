using System.Collections.Generic;
using Chocolatey.Language.Server.Extensions;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate the id does not contain a pre-release tag (alpha, beta, prerelease).
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/PrereleaseVersionAsPartOfPackageIdRequirement.cs">Package validator rule for package id does not contain pre-release tags.</seealso>
    public sealed class PrereleaseVersionAsPartOfPackageId : NuspecRuleBase
    {
        private const string VALIDATION_MESSAGE = "The package id includes a prerelease version name which should be included only in the version of the package.";

        public override string Id => "choco0010";

        public override ValidationType ValidationType => ValidationType.Requirement;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            if (package.Id.ContainsAny("alpha", "beta", "prerelease"))
            {
                yield return CreateDiagnostic(package.Id, VALIDATION_MESSAGE);
            }
        }
    }
}
