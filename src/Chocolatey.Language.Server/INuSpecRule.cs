using System.Collections.Generic;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server
{
    public interface INuSpecRule
    {
        IEnumerable<Diagnostic> Validate(XmlDocumentSyntax syntaxTree, TextPositions textPositions);
    }
}
