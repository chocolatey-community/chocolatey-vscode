using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    /// The base class for making implemention of <see cref="INuspecRule"/> easier.
    /// </summary>
    public abstract class NuspecRuleBase : INuspecRule
    {
        private TextPositions _textPositions;
        private Dictionary<string, string> _ruleUrlMap = new Dictionary<string, string>()
        {
            { "CHOCO0001", "https://gep13.github.io/chocolatey-vscode/docs/rules/CHOCO0001" },
            { "CHOCO0002", "https://gep13.github.io/chocolatey-vscode/docs/rules/CHOCO0002" },
            { "CHOCO0003", "https://gep13.github.io/chocolatey-vscode/docs/rules/CHOCO0003" },
            { "CHOCO0004", "https://gep13.github.io/chocolatey-vscode/docs/rules/CHOCO0004" },
            { "CHOCO1001", "https://gep13.github.io/chocolatey-vscode/docs/rules/CHOCO1001" },
            { "CHOCO1002", "https://gep13.github.io/chocolatey-vscode/docs/rules/CHOCO1002" },
        };

        /// <summary>
        /// Sets the position of the currently used nuspec text.
        /// </summary>
        /// <param name="positions">The positions to use for the current text.</param>
        /// <remarks>
        /// This should be set in <see cref="DiagnosticsHandler"/> before running the <see
        /// cref="M:Chocolatey.Language.Server.Validations.NuspecRuleBase.Validate(XmlDocumentSyntax)"/> method.
        /// </remarks>
        public void SetTextPositions(TextPositions positions)
            => _textPositions = positions;

        /// <summary>
        /// Runs validation of the current nuspec file by using the specified <paramref name="syntaxTree"/>.
        /// </summary>
        /// <param name="syntaxTree">The syntax tree to use during validation.</param>
        /// <returns>An enumerable of failed checks</returns>
        public abstract IEnumerable<Diagnostic> Validate(XmlDocumentSyntax syntaxTree);

        /// <summary>
        /// Finds a single element in the specified <paramref name="syntaxTree"/> by the name.
        /// </summary>
        /// <param name="syntaxTree">The syntax tree to find the element in.</param>
        /// <param name="name">The name of the element.</param>
        /// <returns>The element syntax if one is found; otherwise returns <c>null</c>.</returns>
        protected XmlElementSyntax FindElementByName(XmlDocumentSyntax syntaxTree, string name)
            => syntaxTree.DescendantNodes()
                         .OfType<XmlElementSyntax>()
                         .FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));

        #region Diagnostic creation helpers

        /// <summary>
        ///   Creates a single requirement diagnostic using the specified <paramref name="syntaxTree" />.
        /// </summary>
        /// <param name="elementSyntax">The element syntax.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateRequirement(XmlElementSyntax elementSyntax, string message, string code = null)
            => CreateDiagnostic(elementSyntax, DiagnosticSeverity.Error, message, code);

        /// <summary>
        ///   Creates a single requirement diagnostic using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="syntaxNode">The syntax node.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateRequirement(SyntaxNode syntaxNode, string message, string code = null)
            => CreateDiagnostic(syntaxNode, DiagnosticSeverity.Error, message, code);

        /// <summary>
        ///   Creates a single guideline diagnostic using the specified <paramref name="syntaxTree" />.
        /// </summary>
        /// <param name="elementSyntax">The element syntax.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateGuideline(XmlElementSyntax elementSyntax, string message, string code = null)
            => CreateDiagnostic(elementSyntax, DiagnosticSeverity.Warning, message, code);

        /// <summary>
        ///   Creates a single guideline diagnostic using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="syntaxNode">The syntax node.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateGuideline(SyntaxNode syntaxNode, string message, string code = null)
            => CreateDiagnostic(syntaxNode, DiagnosticSeverity.Warning, message, code);

        /// <summary>
        ///   Creates a single suggestion diagnostic using the specified <paramref name="syntaxTree" />.
        /// </summary>
        /// <param name="elementSyntax">The element syntax.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateSuggestion(XmlElementSyntax elementSyntax, string message, string code = null)
            => CreateDiagnostic(elementSyntax, DiagnosticSeverity.Information, message, code);

        /// <summary>
        ///   Creates a single suggestion diagnostic using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="syntaxNode">The syntax node.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateSuggestion(SyntaxNode syntaxNode, string message, string code = null)
            => CreateDiagnostic(syntaxNode, DiagnosticSeverity.Information, message, code);

        /// <summary>
        ///   Creates a single note diagnostic using the specified <paramref name="syntaxTree" />.
        /// </summary>
        /// <param name="elementSyntax">The element syntax.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateNote(XmlElementSyntax elementSyntax, string message, string code = null)
            => CreateDiagnostic(elementSyntax, DiagnosticSeverity.Hint, message, code);

        /// <summary>
        ///   Creates a single note diagnostic using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="syntaxNode">The syntax node.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateNote(SyntaxNode syntaxNode, string message, string code = null)
            => CreateDiagnostic(syntaxNode, DiagnosticSeverity.Hint, message, code);

        /// <summary>
        ///   Creates a single diagnostic with the specified <paramref name="severity"/> using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="elementSyntax">The element syntax.</param>
        /// <param name="severity">The Diagnostic severity to show the user.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The the created diagnostic.</returns>
        protected Diagnostic CreateDiagnostic(XmlElementSyntax elementSyntax, DiagnosticSeverity severity, string message, string code)
            => CreateDiagnostic(elementSyntax.StartTag.End, elementSyntax.EndTag.Start, severity, message, code);

        /// <summary>
        ///   Creates a single diagnostic with the specified <paramref name="severity"/> using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="syntaxNode">The syntax node.</param>
        /// <param name="severity">The Diagnostic severity to show the user.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The created diagnostic.</returns>
        protected Diagnostic CreateDiagnostic(SyntaxNode syntaxNode, DiagnosticSeverity severity, string message, string code)
            => CreateDiagnostic(syntaxNode.Start, syntaxNode.End, severity, message, code);

        /// <summary>
        /// Creates a single diagnostic with the specified <paramref name="severity"/>.
        /// </summary>
        /// <param name="start">The start of where the diagnostic should be displayed.</param>
        /// <param name="end">The end of where the diagnostic should be displayed.</param>
        /// <param name="severity">The Diagnostic severity to show the user.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The created diagnostic.</returns>
        protected virtual Diagnostic CreateDiagnostic(int start, int end, DiagnosticSeverity severity, string message, string code)
        {
            string errorMessage = string.IsNullOrEmpty(_ruleUrlMap[code]) ?
                                  message :
                                  $"{message}\nSee: {_ruleUrlMap[code]}";

            return new Diagnostic
            {
                Message = errorMessage,
                Severity = severity,
                Range = _textPositions.GetRange(start, end),
                Source = "chocolatey",
                Code = code,
            };
        }

        #endregion Diagnostic creation helpers
    }
}
