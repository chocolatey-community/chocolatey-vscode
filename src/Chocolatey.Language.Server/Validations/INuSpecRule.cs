using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace Chocolatey.Language.Server.Validations
{
    /// <summary>
    /// Interface used to define common methods used when validation a nuspec file.
    /// </summary>
    public interface INuspecRule
    {

        string Id { get; }

        string DocumentationUrl { get; }

        ValidationType ValidationType { get; }

        /// <summary>
        /// Runs validation of the current nuspec file by using the specified <paramref name="syntaxTree"/>.
        /// </summary>
        /// <param name="package">The package to run the validation on.</param>
        /// <returns>An enumerable of failed checks.</returns>
        IEnumerable<Diagnostic> Validate(Models.Package package);
    }
}
