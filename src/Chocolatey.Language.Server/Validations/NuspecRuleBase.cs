using System;
using System.Collections.Generic;
using Chocolatey.Language.Server.Engine;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    /// The base class for making implemention of <see cref="INuspecRule"/> easier.
    /// </summary>
    public abstract class NuspecRuleBase : INuspecRule
    {
        /// <summary>
        /// Gets the string Id for the rule, similar to CHOCO0001
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Gets the documentation Url for the rule
        /// </summary>
        public abstract string DocumentationUrl { get; }

        /// <summary>
        /// Gets the type of of validation
        /// </summary>
        public abstract ValidationType ValidationType { get; }

        internal static TextPositions TextPositions { get; set; }

        /// <summary>
        /// Runs validation of current package.
        /// </summary>
        /// <param name="package">The package to run the validation on.</param>
        /// <returns>An enumerable of failed checks.</returns>
        public abstract IEnumerable<Diagnostic> Validate(Package package);

        /// <summary>
        /// Creates a single diagnostic with the specified <paramref name="severity"/> using the
        /// specified <paramref name="metaValue"/>.
        /// </summary>
        /// <param name="severity">The Diagnostic severity to show the user.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="message">The message.</param>
        /// <returns>The the created diagnostic.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="metaValue"/> is <c>null</c>.
        /// </exception>
        protected Diagnostic CreateDiagnostic(MetaValue metaValue, string message)
        {
            if (metaValue == null)
            {
                throw new ArgumentNullException(nameof(metaValue));
            }

            return CreateDiagnostic(metaValue.ElementStart, metaValue.ElementEnd, message);
        }

        /// <summary>
        /// Creates a single diagnostic with the specified <paramref name="severity"/>.
        /// </summary>
        /// <param name="start">The start of where the diagnostic should be displayed.</param>
        /// <param name="end">The end of where the diagnostic should be displayed.</param>
        /// <param name="severity">The Diagnostic severity to show the user.</param>
        /// <param name="message">The message to show the user.</param>
        /// <returns>The created diagnostic.</returns>
        protected virtual Diagnostic CreateDiagnostic(int start, int end, string message)
        {
            string errorMessage = string.IsNullOrEmpty(DocumentationUrl) ?
                                  message :
                                  $"{message}\nSee: {DocumentationUrl}";

            DiagnosticSeverity severity;

            switch (this.ValidationType)
            {
                case ValidationType.Guideline:
                    severity = DiagnosticSeverity.Warning;
                    break;
                case ValidationType.Suggestion:
                    severity = DiagnosticSeverity.Information;
                    break;
                case ValidationType.Note:
                    severity = DiagnosticSeverity.Hint;
                    break;
                default:
                    severity = DiagnosticSeverity.Error;
                    break;
            }

            return new Diagnostic
            {
                Message = errorMessage,
                Severity = severity,
                Range = TextPositions.GetRange(start, end),
                Source = "chocolatey",
                Code = Id,
            };
        }
    }
}
