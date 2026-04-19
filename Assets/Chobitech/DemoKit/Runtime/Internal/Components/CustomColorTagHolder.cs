// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Collections.Generic;
using System.Linq;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// [Internal] Manages a collection of <see cref="CustomColorTag"/> objects and provides functionality 
    /// to map custom tags to Unity's rich text color format.
    /// This holder serves as a local cache for fast lookup during text processing.
    /// </summary>
    internal class CustomColorTagHolder
    {
        private readonly Dictionary<string, CustomColorTag> _tagMap = new();

        internal CustomColorTag[] ColorTags => _tagMap.Values.ToArray();

        internal CustomColorTagHolder(IEnumerable<CustomColorTag> tags = null)
        {
            if (tags == null)
            {
                return;
            }

            foreach (var t in tags)
            {
                _tagMap[t.tag] = t;
            }
        }

        internal bool IsTagExists(string tag) => _tagMap.ContainsKey(tag);

        internal bool TryGetCustomColorTag(string tag, out CustomColorTag cct)
            => _tagMap.TryGetValue(tag, out cct);

        internal string ConvertCustomTagToRichText(string text)
        {
            return CustomTagController.ConvertCustomTaggedText(
                text,
                data =>
                {
                    if (TryGetCustomColorTag(data.tag, out var lct))
                    {
                        return $"<color={lct.Hex}>{data.content}</color>";
                    }
                    return data.content;
                }
            );
        }
    }
}
