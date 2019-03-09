using System;
using System.Collections.Generic;
using System.Linq;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    public sealed class DeprecatedPackagesShouldHaveDependency : NuspecRuleBase
    {
        private const string VALIDATION_MESSAGE = "Deprecated Packages must contain a dependency to the package(s) which the package is deprecating for.";
        public override string Id => "choco0007";

        public override string DocumentationUrl => $"https://gep13.github.io/chocolatey-vscode/docs/rules/{Id}";

        public override ValidationType ValidationType => ValidationType.Requirement;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            if (package.Title.IsMissing)
            {
                yield break;
            }

            if (package.Title.Value.Contains("deprecated", StringComparison.OrdinalIgnoreCase)
                && !package.Dependencies.Any())
            {
                yield return CreateDiagnostic(package, VALIDATION_MESSAGE);
            }
        }
    }
}
