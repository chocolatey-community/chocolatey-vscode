using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server.Validations
{
    public sealed class CopyrightAndAuthorFieldShouldntContainEmailRequirement : NuspecRuleBase
    {
        // This is the same regex as used in the Package validator
        // Known problems with this regex is it doesn't always match the whole email address.
        private const string EmailRegexPattern = @"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}";

        private static readonly IReadOnlyList<string> ElementsToValidate = new[]
        {
            "copyright",
            "authors"
        };

        private static readonly Regex _emailRegex = new Regex(EmailRegexPattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Gets the string Id for the rule, similar to CHOCO0001
        /// </summary>
        public override string Id
        {
            get
            {
                return "CHOCO0005";
            }
        }

        /// <summary>
        /// Gets the documentation Url for the rule
        /// </summary>
        public override string DocumentationUrl
        {
            get
            {
                return "https://gep13.github.io/chocolatey-vscode/docs/rules/CHOCO0005";
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
        public override IEnumerable<Diagnostic> Validate(XmlDocumentSyntax syntaxTree)
        {
            return syntaxTree.DescendantNodes()
                .OfType<XmlElementSyntax>()
                .Where(x => ElementsToValidate.Any(e => string.Equals(x.Name, e, StringComparison.OrdinalIgnoreCase)))
                .SelectMany(HandleValidation);
        }

        private IEnumerable<Diagnostic> HandleValidation(XmlElementSyntax element)
        {
            var content = element.GetContentValue();

            if (string.IsNullOrWhiteSpace(content))
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var emailMatches = _emailRegex.Matches(content);

            return emailMatches
                .Where(e => e.Success)
                .Select(e =>
                {
                    int start = element.StartTag.End + e.Index;
                    int end = start + e.Length;
                    return CreateDiagnostic(
                        start,
                        end,
                        DiagnosticSeverity.Error,
                        $"Email address should not be used in the {element.Name} field.");
                });
        }
    }
}
