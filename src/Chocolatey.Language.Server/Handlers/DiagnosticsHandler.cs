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
using Chocolatey.Language.Server.Extensions;
using Chocolatey.Language.Server.Engine;
using Chocolatey.Language.Server.Validations;

namespace Chocolatey.Language.Server.Handlers
{
    public class DiagnosticsHandler
    {
        private readonly ILanguageServer _router;
        private readonly BufferManager _bufferManager;
        private IList<INuspecRule> _rules = new List<INuspecRule>();
        private IConfigurationProvider _configurationProvider;

        public DiagnosticsHandler(ILanguageServer router, BufferManager bufferManager, IEnumerable<INuspecRule> rules, IConfigurationProvider configurationProvider)
        {
            _router = router;
            _bufferManager = bufferManager;
            _rules = new List<INuspecRule>(rules);
            _configurationProvider = configurationProvider;
        }

        public void PublishDiagnostics(Uri uri, Buffer buffer)
        {
            var text = buffer.GetText(0, buffer.Length);
            var syntaxTree = Parser.Parse(buffer);
            var textPositions = new TextPositions(text);
            var diagnostics = new List<Diagnostic>();

            foreach (var rule in _rules.OrEmptyListIfNull())
            {
                rule.SetTextPositions(textPositions);
                diagnostics.AddRange(rule.Validate(syntaxTree));
            }

            _router.Document.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = uri,
                Diagnostics = diagnostics
            });
        }
    }
}
