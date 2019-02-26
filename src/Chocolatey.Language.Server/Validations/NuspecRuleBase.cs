﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    /// The base class for making implemention of <see cref="INuSpecRule"/> easier.
    /// </summary>
    public abstract class NuspecRuleBase : INuSpecRule
    {
        private TextPositions _textPositions;

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
        /// <param name="wikiUrl">The wiki URL for the current rule.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateRequirement(XmlElementSyntax elementSyntax, string message, string wikiUrl = null, string code = null)
            => CreateDiagnostic(elementSyntax, DiagnosticSeverity.Error, message, wikiUrl, code);

        /// <summary>
        ///   Creates a single requirement diagnostic using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="syntaxNode">The syntax node.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="wikiUrl">The wiki URL for the current rule.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateRequirement(SyntaxNode syntaxNode, string message, string wikiUrl = null, string code = null)
            => CreateDiagnostic(syntaxNode, DiagnosticSeverity.Error, message, wikiUrl, code);

        /// <summary>
        ///   Creates a single guideline diagnostic using the specified <paramref name="syntaxTree" />.
        /// </summary>
        /// <param name="elementSyntax">The element syntax.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="wikiUrl">The wiki URL for the current rule.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateGuideline(XmlElementSyntax elementSyntax, string message, string wikiUrl = null, string code = null)
            => CreateDiagnostic(elementSyntax, DiagnosticSeverity.Warning, message, wikiUrl, code);

        /// <summary>
        ///   Creates a single guideline diagnostic using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="syntaxNode">The syntax node.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="wikiUrl">The wiki URL for the current rule.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateGuideline(SyntaxNode syntaxNode, string message, string wikiUrl = null, string code = null)
            => CreateDiagnostic(syntaxNode, DiagnosticSeverity.Warning, message, wikiUrl, code);

        /// <summary>
        ///   Creates a single suggestion diagnostic using the specified <paramref name="syntaxTree" />.
        /// </summary>
        /// <param name="elementSyntax">The element syntax.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="wikiUrl">The wiki URL for the current rule.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateSuggestion(XmlElementSyntax elementSyntax, string message, string wikiUrl = null, string code = null)
            => CreateDiagnostic(elementSyntax, DiagnosticSeverity.Information, message, wikiUrl, code);

        /// <summary>
        ///   Creates a single suggestion diagnostic using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="syntaxNode">The syntax node.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="wikiUrl">The wiki URL for the current rule.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateSuggestion(SyntaxNode syntaxNode, string message, string wikiUrl = null, string code = null)
            => CreateDiagnostic(syntaxNode, DiagnosticSeverity.Information, message, wikiUrl, code);

        /// <summary>
        ///   Creates a single note diagnostic using the specified <paramref name="syntaxTree" />.
        /// </summary>
        /// <param name="elementSyntax">The element syntax.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="wikiUrl">The wiki URL for the current rule.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateNote(XmlElementSyntax elementSyntax, string message, string wikiUrl = null, string code = null)
            => CreateDiagnostic(elementSyntax, DiagnosticSeverity.Hint, message, wikiUrl, code);

        /// <summary>
        ///   Creates a single note diagnostic using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="syntaxNode">The syntax node.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="wikiUrl">The wiki URL for the current rule.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The requirement diagnostic.</returns>
        protected Diagnostic CreateNote(SyntaxNode syntaxNode, string message, string wikiUrl = null, string code = null)
            => CreateDiagnostic(syntaxNode, DiagnosticSeverity.Hint, message, wikiUrl, code);

        /// <summary>
        ///   Creates a single diagnostic with the specified <paramref name="severity"/> using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="elementSyntax">The element syntax.</param>
        /// <param name="severity">The Diagnostic severity to show the user.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="wikiUrl">The wiki URL for the current rule.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The the created diagnostic.</returns>
        protected Diagnostic CreateDiagnostic(XmlElementSyntax elementSyntax, DiagnosticSeverity severity, string message, string wikiUrl, string code)
            => CreateDiagnostic(elementSyntax.StartTag.End, elementSyntax.EndTag.Start, severity, message, wikiUrl, code);

        /// <summary>
        ///   Creates a single diagnostic with the specified <paramref name="severity"/> using the specified <paramref name="syntaxNode" />.
        /// </summary>
        /// <param name="syntaxNode">The syntax node.</param>
        /// <param name="severity">The Diagnostic severity to show the user.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="wikiUrl">The wiki URL for the current rule.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The created diagnostic.</returns>
        protected Diagnostic CreateDiagnostic(SyntaxNode syntaxNode, DiagnosticSeverity severity, string message, string wikiUrl, string code)
            => CreateDiagnostic(syntaxNode.Start, syntaxNode.End, severity, message, wikiUrl, code);

        /// <summary>
        /// Creates a single diagnostic with the specified <paramref name="severity"/>.
        /// </summary>
        /// <param name="start">The start of where the diagnostic should be displayed.</param>
        /// <param name="end">The end of where the diagnostic should be displayed.</param>
        /// <param name="severity">The Diagnostic severity to show the user.</param>
        /// <param name="message">The message to show the user.</param>
        /// <param name="wikiUrl">The wiki URL for the current rule.</param>
        /// <param name="code">The code for the current rule.</param>
        /// <returns>The created diagnostic.</returns>
        protected virtual Diagnostic CreateDiagnostic(int start, int end, DiagnosticSeverity severity, string message, string wikiUrl, string code)
        {
            string errorMessage = string.IsNullOrEmpty(wikiUrl) ?
                                  message :
                                  $"{message}\nSee: {wikiUrl}";

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
