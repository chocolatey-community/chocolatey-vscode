using System;
using System.Collections.Generic;
using System.Linq;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate that no tags contain a comma.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/TagsAreSpaceSeparatedRequirement.cs">Package validator comma separated tags validation rule.</seealso>
    public sealed class TagsAreSpaceSeparated : NuspecRuleBase
    {
        private const string VALIDATION_MESSAGE = "Tags (tags) are space separated values for referencing categories for software. Please don't use comma to separate tags.";

        public override string Id => "CHOCO0012";

        public override string DocumentationUrl => $"https://gep13.github.io/chocolatey-vscode/docs/rules/{Id}";

        public override ValidationType ValidationType => ValidationType.Requirement;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            if (!package.Tags.Any())
            {
                yield break;
            }

            foreach (var tag in package.Tags)
            {
                if (tag.Value.Contains(',', StringComparison.OrdinalIgnoreCase))
                {
                    yield return CreateDiagnostic(tag, VALIDATION_MESSAGE, true);
                }
            }
        }
    }
}
