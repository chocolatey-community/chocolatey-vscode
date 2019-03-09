using System;
using System.Collections.Generic;
using System.Linq;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate the nuspec enhancements
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/NuspecEnhancementsMissingSuggestion.cs">Package validator requirement for nuspec enhancements.</seealso>
    public class NuspecEnhancementsValidation : NuspecRuleBase
    {
        /// <summary>
        /// Gets the string Id for the rule, similar to choco0001
        /// </summary>
        public override string Id
        {
            get
            {
                return "choco2001";
            }
        }

        /// <summary>
        /// Gets the type of of validation
        /// </summary>
        public override ValidationType ValidationType
        {
            get
            {
                return ValidationType.Suggestion;
            }
        }

        private static readonly IReadOnlyCollection<string> NuspecEnhancements = new []
        {
            "bugTrackerUrl",
            "docsUrl",
            "mailingListUrl",
            "projectSourceUrl",
        };

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            var elements = NuspecEnhancements.Where(x => !package.AllElements.Any(u => string.Equals(u.Key, x, StringComparison.OrdinalIgnoreCase)));

            foreach (var element in elements) {
                yield return CreateDiagnostic(
                    package,
                    $"The nuspec has been enhanced to allow more information related to the software. Please add an {element} if there is one."
                );
            }
        }
    }
}
