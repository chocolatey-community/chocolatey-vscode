using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    /// Runs validation of the current nuspec to verify the dependency on the
    /// chocolatey package ID.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/IncludesChocolateyDependencyNote.cs">Package validator note for taking a dependency on Chocolatey.</seealso>
    public sealed class IncludesChocolateyDependencyNote : NuspecRuleBase
    {
        /// <inheritdoc />
        /// <summary>
        /// Gets the string Id for the rule, similar to CHOCO0001
        /// </summary>
        public override string Id => "choco3003";

        /// <inheritdoc />
        /// <summary>
        /// Gets the type of of validation
        /// </summary>
        public override ValidationType ValidationType => ValidationType.Note;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            var dependency = package.Dependencies.Any(x =>
                string.Equals(x.Value.Id, "chocolatey", StringComparison.OrdinalIgnoreCase));
            if (dependency)
            {
                yield return CreateDiagnostic(
                    package.Dependencies.First().TextStart,
                    package.Dependencies.Last().TextEnd,
                    "The package takes a dependency on Chocolatey. The reviewer will ensure the package uses a specific Chocolatey feature that requires a minimum version."
                );
            }
        }
    }
}
