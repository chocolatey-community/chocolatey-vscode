using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    /// Runs validation of the current nuspec to verify that the author and owner
    /// do not match.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/AuthorDoesNotMatchMaintainerNote.cs">Package validator note for author and owner values.</seealso>
    public sealed class AuthorDoesNotMatchMaintainer : NuspecRuleBase
    {
        /// <inheritdoc />
        /// <summary>
        /// Gets the string Id for the rule, similar to CHOCO0001
        /// </summary>
        public override string Id => "choco3002";

        /// <inheritdoc />
        /// <summary>
        /// Gets the type of of validation
        /// </summary>
        public override ValidationType ValidationType => ValidationType.Note;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            var owners = string.Join(",", package.Maintainers).ToLower();
            var authors = string.Join(",", package.Authors).ToLower();
            if (package.Maintainers.Count <= 0)
            {
                yield break;
            }

            if (owners.Equals(authors))
            {
                yield return CreateDiagnostic(
                    package.Maintainers.First().TextStart,
                    package.Maintainers.Last().TextEnd,
                    "The package maintainer field (owners) matches the software author field (authors) in the nuspec. The reviewer will ensure that the package maintainer is also the software author."
                );
                yield return CreateDiagnostic(
                    package.Authors.First().TextStart,
                    package.Authors.Last().TextEnd,
                    "The package maintainer field (owners) matches the software author field (authors) in the nuspec. The reviewer will ensure that the package maintainer is also the software author."
                );
            }
        }
    }
}
