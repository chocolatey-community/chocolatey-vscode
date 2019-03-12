using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var package = Parsers.PackageParser.ParseXmlDocument(syntaxTree);
            Validations.NuspecRuleBase.TextPositions = textPositions;

            var diagnostics = _rules.OrEmptyListIfNull()
                .AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism)
                .Where(r => !IsSuppressedRule(r.Id))
                .SelectMany(r => r.Validate(package));

            _router.Document.PublishDiagnostics(new PublishDiagnosticsParams
            {
                Uri = uri,
                Diagnostics = diagnostics.ToList()
            });

            stopwatch.Stop();

            _router.Window.LogInfo("Rules processing in: " + stopwatch.ElapsedMilliseconds + " milliseconds");
        }

        private bool IsSuppressedRule(string ruleId)
        {
            return _configurationProvider.Configuration.Language.SuppressedRules.Contains(ruleId);
        }
    }
}
