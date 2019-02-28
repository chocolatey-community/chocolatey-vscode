
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Chocolatey.Language.Server
{
    public class CodeActionHandler : ICodeActionHandler
    {
        private CodeActionCapability _capability;

        public TextDocumentRegistrationOptions GetRegistrationOptions()
        {
            return new TextDocumentRegistrationOptions
            {
                DocumentSelector = new DocumentSelector(new DocumentFilter { Pattern = "**/*.nuspec" } )
            };
        }

        public Task<CommandOrCodeActionContainer> Handle(CodeActionParams request, CancellationToken cancellationToken)
        {
            var container = request.Context.Diagnostics?
                .Where(diagnostic => diagnostic.Source.Equals("chocolatey", StringComparison.OrdinalIgnoreCase) &&
                                     diagnostic.Code.IsString && !string.IsNullOrWhiteSpace(diagnostic.Code.String))
                .Select(diagnostic => {
                    var url = $"https://gep13.github.io/chocolatey-vscode/docs/rules/{diagnostic.Code.String}";
                    var title = $"Click for more information {url}";
                    return new CommandOrCodeAction(new CodeAction {
                        Title = title,
                        Diagnostics = new [] { diagnostic },
                        Kind = CodeActionKind.QuickFix,
                        Command = new Command {
                            Name = "chocolatey.open",
                            Title = title,
                            Arguments = new JArray(url)
                        }
                    });
                }).ToArray() ?? Array.Empty<CommandOrCodeAction>();

            return Task.FromResult(new CommandOrCodeActionContainer(container));
        }

        public void SetCapability(CodeActionCapability capability)
        {
            _capability = capability;
        }
    }
}
