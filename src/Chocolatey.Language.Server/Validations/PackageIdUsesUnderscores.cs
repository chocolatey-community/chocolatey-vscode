using System.Collections.Generic;
using Chocolatey.Language.Server.Extensions;
using Chocolatey.Language.Server.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary> Handler to validate if the package id contains an underscore ('_'). </summary>
    /// <seealso
    /// href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/PackageIdUsesUnderscoresNote.cs">Package
    /// validator package id contains underscore rule.
    public sealed class PackageIdUsesUnderscores : NuspecRuleBase
    {
        private const string VALIDATION_MESSAGE = "The package id contains underscores (_). Usually the package id is separated by '-' instead of underscores. Please change the underscores to '-' if this is a new package.";

        public override string Id => "choco3001";

        public override string DocumentationUrl => "Should be removed after PR #174";

        public override ValidationType ValidationType => ValidationType.Note;

        public override IEnumerable<Diagnostic> Validate(Package package)
        {
            if (package.Id.ContainsAny("_"))
            {
                yield return CreateDiagnostic(package.Id, VALIDATION_MESSAGE, true);
            }
        }
    }
}
