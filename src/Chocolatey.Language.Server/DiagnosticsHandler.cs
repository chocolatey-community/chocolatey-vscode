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

namespace Chocolatey.Language.Server
{
    public class DiagnosticsHandler
    {
        private readonly ILanguageServer _router;
        private readonly BufferManager _bufferManager;
        private IList<INuSpecRule> _rules = new List<INuSpecRule>();

        public DiagnosticsHandler(ILanguageServer router, BufferManager bufferManager)
        {
            _router = router;
            _bufferManager = bufferManager;

            var typeLocator = new TypeLocator();
            foreach (var nuspecRule in typeLocator.GetTypesThatInheritOrImplement<INuSpecRule>().OrEmptyListIfNull())
            {
                var rule = Activator.CreateInstance(nuspecRule) as INuSpecRule;
                if (rule != null)
                {
                    _rules.Add(rule);
                }
            }
        }

        public void PublishDiagnostics(Uri uri, Buffer buffer)
        {
            var text = buffer.GetText(0, buffer.Length);
            var syntaxTree = Parser.Parse(buffer);
            var textPositions = new TextPositions(text);
            var diagnostics = new List<Diagnostic>();

            foreach (var rule in _rules.OrEmptyListIfNull())
            {
                diagnostics.AddRange(rule.Validate(syntaxTree, textPositions));
            }

            _router.Document.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = uri,
                Diagnostics = diagnostics
            });
        }
    }
}
