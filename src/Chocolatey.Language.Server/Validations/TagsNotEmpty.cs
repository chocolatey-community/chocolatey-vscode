using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate that no tags contain a comma.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/TagsAreSpaceSeparatedRequirement.cs">Package validator comma separated tags validation rule.</seealso>
    public sealed class TagsNotEmpty : NuspecRuleBase
    {
        private const string VALIDATION_MESSAGE = "Tags (tags) are space separated values for referencing categories for software. " +
                                                  "Please include tags in the nuspec as space separated values";

        public override string Id => "choco0013";

        public override ValidationType ValidationType => ValidationType.Requirement;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            if (!package.Tags.Any() || package.Tags.All(t => string.IsNullOrWhiteSpace(t)))
            {
                yield return CreateDiagnostic(package, VALIDATION_MESSAGE);
            }
        }
    }
}
