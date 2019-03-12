using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server.Validations
{
    public sealed class CopyrightAndAuthorFieldShouldntContainEmailRequirement : NuspecRuleBase
    {
        // This is the same regex as used in the Package validator
        private const string EmailRegexPattern = @"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}";

        private static readonly Regex _emailRegex = new Regex(EmailRegexPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets the string Id for the rule, similar to choco0001
        /// </summary>
        public override string Id
        {
            get
            {
                return "choco0005";
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

        /// <summary>
        /// Runs validation of the current nuspec file by using the specified <paramref
        /// name="syntaxTree"/> to check if author or copyright contains an email address.
        /// </summary>
        /// <param name="syntaxTree">The syntax tree to use during validation.</param>
        /// <returns>An enumerable of failed checks</returns>
        /// <seealso cref="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/CopyRightAndAuthorFieldsShouldntContainEmailRequirement.cs">
        /// Package validator requirement for email values
        /// </seealso>
        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            return ValidateText(package.Copyright, "Copyright")
                .Concat(package.Authors.SelectMany(a => ValidateText(a, "Authors")));
        }

        private IEnumerable<Diagnostic> ValidateText(MetaValue<string> metaValue, string name)
        {
            if (string.IsNullOrWhiteSpace(metaValue))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var emailMatches = _emailRegex.Matches(metaValue);

            return emailMatches
                .Where(e => e.Success)
                .Select(e =>
                {
                    int start = metaValue.TextStart + e.Index;
                    int end = start + e.Length;
                    return CreateDiagnostic(
                        start,
                        end,
                        $"Email address should not be used in the {name} field.");
                });
        }
    }
}
