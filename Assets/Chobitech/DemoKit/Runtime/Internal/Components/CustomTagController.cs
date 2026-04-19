// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System;
using System.Text.RegularExpressions;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// [Internal] Static controller responsible for parsing and converting custom text tags (e.g., [tag]content[/tag]).
    /// It uses regular expressions to identify tags and provides hooks for custom string conversion,
    /// primarily used for mapping custom identifiers to Unity rich text.
    /// </summary>
    internal static class CustomTagController
    {
        internal readonly struct CustomTagData
        {
            internal readonly string tag;

            internal readonly string content;

            internal CustomTagData(string tag, string content)
            {
                this.tag = tag;
                this.content = content;
            }
        }

        private static readonly Regex CustomTagRegex = new(@"\[(?<tag>[0-9a-z]+)\](?<content>.*?)\[/\k<tag>\]", RegexOptions.Singleline);

        internal static string GetCustomTaggedText(string tag, string content)
        {
            return $"[{tag}]{content}[/{tag}]";
        }

        internal static string ConvertCustomTaggedText(string input, Func<CustomTagData, string> converter)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            return CustomTagRegex.Replace(
                input,
                m =>
                {
                    var tag = m.Groups["tag"].Value;
                    var content = m.Groups["content"].Value;
                    return converter(new(tag, content));
                }
            );
        }
    }
}
