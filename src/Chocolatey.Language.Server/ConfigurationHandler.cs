using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace Chocolatey.Language.Server
{
    public class ConfigurationHandler : IDidChangeConfigurationHandler
    {
        /// <summary>
        ///     The client-side capabilities for DidChangeConfiguration.
        /// </summary>
        public DidChangeConfigurationCapability Capabilities { get; private set; }

        public object GetRegistrationOptions()
        {
            return null;
        }

        public Task<Unit> Handle(DidChangeConfigurationParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public void SetCapability(DidChangeConfigurationCapability capability)
        {
            Capabilities = capability;
        }
    }
}
