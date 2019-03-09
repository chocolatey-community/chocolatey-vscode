using System;
using Chocolatey.Language.Server.Models;

namespace Chocolatey.Language.Server.Extensions
{
    /// <summary>
    /// Helper class to make it easier to test out values in the Meta Value class.
    /// </summary>
    public static class MetaValueExtensions
    {
        public static bool ContainsAny(this MetaValue<string> source, params string[] values)
        {
            if (source.IsMissing)
            {
                return false;
            }

            foreach (var value in values)
            {
                if (source.Value.Contains(value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool EndsWith(this MetaValue<string> source, string text)
        {
            return !source.IsMissing && source.Value.EndsWith(text, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsNullOrWhitespace(this MetaValue<string> source)
        {
            return source.IsMissing || string.IsNullOrWhiteSpace(source);
        }
    }
}
