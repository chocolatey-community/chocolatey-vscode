using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Chocolatey.Language.Server.Models;
using Microsoft.Language.Xml;

namespace Chocolatey.Language.Server.Parsers
{
    /// <summary>
    /// Helper class for parsing a package.
    /// </summary>
    public static class PackageParser
    {
        /// <summary>
        /// Responsible for parsing the passed in nuspec xml document. This will then result in a
        /// parsed package from that xml.
        /// </summary>
        /// <param name="document">The nuspec xml document to parse.</param>
        /// <returns>The full chocolatey package that was parsed from the specified xml document.</returns>
        public static Package ParseXmlDocument(XmlDocumentSyntax document)
        {
            var package = new Package();
            var allElements = new SortedDictionary<string, MetaValue<string>>();

            foreach (var element in document.DescendantNodes().OfType<XmlElementSyntax>())
            {
                switch (element.Name.ToLowerInvariant())
                {
                    case "owners":
                        package.Maintainers = ReadDelimitedList(element, ',');
                        break;

                    case "authors":
                        package.Authors = ReadDelimitedList(element, ',');
                        break;

                    case "tags":
                        package.Tags = ReadDelimitedList(element, ' ');
                        break;

                    case "metadata":
                        package.StartsAt = element.Start;
                        package.EndsAt = element.StartTag.End;
                        break;

                    case "dependencies":
                        var descendants = element.DescendantNodes().OfType<XmlEmptyElementSyntax>();
                        AddDependencies(descendants, package);
                        break;

                    case "package":
                    case "files":
                        break;

                    default:
                        SetObjectProperty(package, element);
                        break;
                }

                AppendElementToDictionary (allElements, element);
            }

            package.AllElements = allElements;

            return package;
        }

        private static void AddDependencies(IEnumerable<XmlEmptyElementSyntax> dependencyElements, Package package)
        {
            foreach (var element in dependencyElements)
            {
                if (string.Compare(element.Name, "dependency", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var dependency = ParseDependency(element);
                    if (!string.IsNullOrEmpty(dependency.Value.Id))
                    {
                        package.AddDependency(ParseDependency(element));
                    }
                }
            }
        }

        private static void AppendElementToDictionary(SortedDictionary<string, MetaValue<string>> allElements, XmlElementSyntax element)
        {
            if (!element.Elements.Any())
            {
                var metaValue = new MetaValue<string>(element.GetContentValue())
                {
                    ElementEnd = element.End,
                    ElementStart = element.Start,
                    TextEnd = element.EndTag.Start,
                    TextStart = element.StartTag.End,
                    IsMissing = false
                };
                allElements.Add(element.Name, metaValue);
            }
        }

        private static MetaValue<Dependency> ParseDependency(XmlEmptyElementSyntax element)
        {
            var dependency = new Dependency();

            SetAttributeMetaValue(dependency.Id, element, "id");
            SetAttributeMetaValue(dependency.VersionRange, element, "version");

            return new MetaValue<Dependency>(dependency)
            {
                IsMissing = false,
                ElementStart = element.Start,
                ElementEnd = element.End
            };
        }

        private static IReadOnlyList<MetaValue<string>> ReadDelimitedList(XmlElementSyntax text, char delimiter)
        {
            var contentValue = text.GetContentValue();
            if (string.IsNullOrWhiteSpace(contentValue)) { return new MetaValue<string>[0]; }

            var values = contentValue.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var listResult = new List<MetaValue<string>>();

            foreach (var value in values)
            {
                int start = text.StartTag.End + contentValue.IndexOf(value);
                int end = start + value.Length;
                listResult.Add(new MetaValue<string>
                {
                    ElementEnd = text.Start,
                    ElementStart = text.End,
                    TextEnd = end,
                    TextStart = start,
                    Value = value,
                    IsMissing = false
                });
            }

            return listResult.AsReadOnly();
        }

        private static void SetAttributeMetaValue(MetaValue<string> metaValue, XmlEmptyElementSyntax element, string attributeName)
        {
            var attribute = element.GetAttribute(attributeName);
            if (attribute != null)
            {
                metaValue.Value = attribute.Value;
                metaValue.ElementStart = attribute.Start;
                metaValue.ElementEnd = attribute.End;
                metaValue.TextStart = attribute.ValueNode.Start;
                metaValue.TextEnd = attribute.ValueNode.End;
                metaValue.IsMissing = false;
            }
            else
            {
                metaValue.ElementStart = element.Start;
                metaValue.ElementEnd = element.End;
            }
        }
        private static void SetObjectProperty(object obj, XmlElementSyntax element)
        {
            var property = obj.GetType().GetProperty(element.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.IgnoreCase);

            var contentValue = element.GetContentValue();
            if (!(property.GetValue(obj) is MetaValue value)) { return; }
            value.ElementEnd = element.End;
            value.ElementStart = element.Start;
            value.IsMissing = false;
            value.TextEnd = element.StartTag.End;
            value.TextStart = element.EndTag.Start;

            if (value is MetaValue<string> stringValue)
            {
                stringValue.Value = contentValue;
            }
            else if (value is MetaValue<bool> boolValue && bool.TryParse(contentValue, out bool result))
            {
                boolValue.Value = result;
            }
        }
    }
}
