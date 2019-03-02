using System.Collections.Generic;
using Chocolatey.Language.Server.Engine;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    /// Interface used to define common methods used when validation a nuspec file.
    /// </summary>
    public interface INuspecRule
    {
        /// <summary>
        /// Sets the position of the currently used nuspec text.
        /// </summary>
        /// <param name="positions">The positions to use for the current text.</param>
        /// <remarks>
        ///   This should be set in <see cref="DiagnosticsHandler"/> before running the <see cref="Validate(XmlDocumentSyntax)"/> method.
        /// </remarks>
        void SetTextPositions(TextPositions positions);

        /// <summary>
        /// Runs validation of the current nuspec file by using the specified <paramref name="syntaxTree"/>.
        /// </summary>
        /// <param name="syntaxTree">The syntax tree to use during validation.</param>
        /// <returns>An enumerable of failed checks</returns>
        IEnumerable<Diagnostic> Validate(XmlDocumentSyntax syntaxTree);

        string Id { get; }

        string DocumentationUrl { get ;}
    }
}
