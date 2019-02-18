using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Language.Xml;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DiagnosticSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    ///   Handler to validate that no templated values remain in the nuspec.
    /// </summary>
    /// <seealso href="https://github.com/chocolatey/package-validator/blob/master/src/chocolatey.package.validator/infrastructure.app/rules/NuspecDoesNotContainTemplatedValuesRequirement.cs">Package validator requirement for templated values.</seealso>
    public class DoesNotContainTemplatedValues : NuspecRuleBase
    {
        private static readonly IReadOnlyCollection<string> TemplatedValues = new []
        {
            "__replace",
            "space_separated",
            "tag1"
        };

        public override IEnumerable<Diagnostic> Validate(XmlDocumentSyntax syntaxTree)
        {
            foreach (var node in syntaxTree.DescendantNodesAndSelf().OfType<XmlTextSyntax>())
            {
                if (!TemplatedValues.Any(x => node.Value.Contains(x, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }

                yield return CreateRequirement(
                    node,
                    "Templated value which should be removed.",
                    "https://github.com/chocolatey/package-validator/wiki/NuspecDoesNotContainTemplatedValues");
            }
        }
    }
}
