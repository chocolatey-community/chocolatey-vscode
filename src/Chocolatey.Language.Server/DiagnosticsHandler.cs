using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Buffer = Microsoft.Language.Xml.Buffer;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server
{
    public class DiagnosticsHandler
    {
        private readonly ILanguageServer _router;
        private readonly BufferManager _bufferManager;
        private static readonly IReadOnlyCollection<string> TemplatedValues = new []
        {
            "__replace",
            "space_separated",
            "tag1"
        };

        public DiagnosticsHandler(ILanguageServer router, BufferManager bufferManager)
        {
            _router = router;
            _bufferManager = bufferManager;
        }

        public void PublishDiagnostics(Uri uri, Buffer buffer)
        {
            var text = buffer.GetText(0, buffer.Length);
            var syntaxTree = Parser.Parse(buffer);
            var textPositions = new TextPositions(text);
            var diagnostics = new List<Diagnostic>();

            diagnostics.AddRange(NuspecDoesNotContainTemplatedValuesRequirement(syntaxTree, textPositions));
            diagnostics.AddRange(NuspecDescriptionLengthRequirement(syntaxTree, textPositions));

            _router.Document.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = uri,
                Diagnostics = diagnostics
            });
        }

        private IEnumerable<Diagnostic> NuspecDoesNotContainTemplatedValuesRequirement(XmlDocumentSyntax syntaxTree, TextPositions textPositions)
        {
            foreach (var node in syntaxTree.DescendantNodesAndSelf().OfType<XmlTextSyntax>())
            {
                if (!TemplatedValues.Any(x => node.Value.Contains(x, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                var range = textPositions.GetRange(node.Start, node.End);

                yield return new Diagnostic {
                    Message = "Templated value which should be removed",
                    Severity = DiagnosticSeverity.Error,
                    Range = range
                };
            }
        }

        private IEnumerable<Diagnostic> NuspecDescriptionLengthRequirement(XmlDocumentSyntax syntaxTree, TextPositions textPositions)
        {
            var descriptionElement = syntaxTree.DescendantNodes().OfType<XmlElementSyntax>().FirstOrDefault(x => string.Equals(x.Name, "description", StringComparison.OrdinalIgnoreCase));

            var descriptionLength = descriptionElement?.GetContentValue().Trim().Length ?? 0;

            if (descriptionLength == 0)
            {
                var range = textPositions.GetRange(descriptionElement?.StartTag.End ?? 0, descriptionElement?.EndTag.Start ?? 0);

                yield return new Diagnostic {
                    Message = "Description is required.",
                    Severity = DiagnosticSeverity.Error,
                    Range = range
                };
            }
        }
    }
}
