using System.Collections.Generic;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    /// Handler to validate the length of copyright in the metadata.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/CopyrightWordCountMinimum4Requirement.cs">
    /// Package validator rule for minimum copyright length
    /// </seealso>
    public sealed class CopyrightWordCount : NuspecRuleBase
    {
        private const string VALIDATION_MESSAGE = "If you are going to use copyright in nuspec, please use more than 4 characters.";

        public override string Id => "choco0006";

        public override string DocumentationUrl => $"https://gep13.github.io/chocolatey-vscode/docs/rules/{Id}";

        public override ValidationType ValidationType => ValidationType.Requirement;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            if (!string.IsNullOrWhiteSpace(package.Copyright) && package.Copyright.Value.Length <= 4)
            {
                yield return CreateDiagnostic(package.Copyright, VALIDATION_MESSAGE);
            }
        }
    }
}
