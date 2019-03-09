using System.Collections.Generic;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    /// Handler to validate that the licenseUrl is being used when requireLicenseAcceptance is set to true.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/LicenseUrlMissingWhenLicenseAcceptanceTrueRequirement.cs">
    /// Package validator rule for license url being used when license acceptance is true.
    /// </seealso>
    public sealed class LicenseUrlMissingWhenLicenseAcceptanceTrue : NuspecRuleBase
    {
        private const string VALIDATION_MESSAGE = "A licenseUrl must bet set when requireLicenseAcceptance is true.";

        public override string Id => "choco0008";

        public override string DocumentationUrl => $"https://gep13.github.io/chocolatey-vscode/docs/rules/{Id}";

        public override ValidationType ValidationType => ValidationType.Requirement;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            if (package.RequireLicenseAcceptance && string.IsNullOrWhiteSpace(package.LicenseUrl))
            {
                var metaValueToUse = package.LicenseUrl.IsMissing ?
                                     (MetaValue)package.RequireLicenseAcceptance :
                                     package.LicenseUrl;
                yield return CreateDiagnostic(
                    metaValueToUse,
                    VALIDATION_MESSAGE);
            }
        }
    }
}
