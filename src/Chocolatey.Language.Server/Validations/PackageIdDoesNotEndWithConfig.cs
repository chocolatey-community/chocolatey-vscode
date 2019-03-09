using System;
using System.Collections.Generic;
using Chocolatey.Language.Server.Extensions;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate the id does not end with .config.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/PackageIdDoesNotEndWithConfigRequirement.cs">Package validator rule for package id does not end with .config.</seealso>
    public sealed class PackageIdDoesNotEndWithConfig : NuspecRuleBase
    {
        private const string VALIDATION_MESSAGE = "The package id ends with .config, this is a reserved file extension, and should not be used.";

        public override string Id => "choco0009";

        public override string DocumentationUrl => $"https://gep13.github.io/chocolatey-vscode/docs/rules/{Id}";

        public override ValidationType ValidationType => ValidationType.Requirement;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            if (package.Id.EndsWith(".config"))
            {
                yield return CreateDiagnostic(package.Id, VALIDATION_MESSAGE);
            }
        }
    }
}
