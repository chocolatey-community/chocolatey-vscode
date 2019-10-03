using System;
using System.Collections.Generic;
using Chocolatey.Language.Server.Extensions;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    /// Handler to validate that a icon url uses either a png or svg file extension.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/CopyrightWordCountMinimum4Requirement.cs">
    /// Package validator rule for validating icon url being png or svg.
    /// </seealso>
    /// <seealso href="https://github.com/chocolatey/choco/wiki/CreatePackages#package-icon-guidelines">
    /// Package creation guidelines
    /// </seealso>
    public sealed class IconUrlShouldUsePngOrSvg : NuspecRuleBase
    {
        private const string VALIDATION_MESSAGE = "As per the packaging guidelines, icons should be either a png or svg file.";

        public override string Id => "choco2002";

        public override ValidationType ValidationType => ValidationType.Suggestion;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            if (package.IconUrl.IsNullOrWhitespace())
            {
                yield break;
            }

            if (!package.IconUrl.EndsWith(".png") && !package.IconUrl.EndsWith(".svg"))
            {
                yield return CreateDiagnostic(package.IconUrl, VALIDATION_MESSAGE);
            }
        }
    }
}
