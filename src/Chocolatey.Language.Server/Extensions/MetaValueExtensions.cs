using System;
using Chocolatey.Language.Server.Models;

namespace Chocolatey.Language.Server.Extensions
{
    /// <summary>
    /// Helper class to make it easier to test out values in the Meta Value class.
    /// </summary>
    public static class MetaValueExtensions
    {
        public static bool EndsWith(this MetaValue<string> source, string text)
        {
            return !source.IsMissing && source.Value.EndsWith(text, StringComparison.OrdinalIgnoreCase);
        }
    }
}
