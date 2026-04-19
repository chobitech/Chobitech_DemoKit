// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// A ScriptableObject that defines a custom color mapping for text tags.
    /// Used to associate a specific string tag with a color for rich text processing within the DemoKit log system.
    /// </summary>
    [CreateAssetMenu(menuName = "Chobitech/DemoKit/Custom Color Tag", fileName = "NewCustomColorTag")]
    public class CustomColorTag : ScriptableObject
    {
        /// <summary>
        /// The string identifier for the tag (e.g., "highlight", "warning").
        /// This tag is used in logs to apply the color defined in this asset.
        /// </summary>
        public string tag;

        /// <summary>
        /// The color associated with this tag.
        /// </summary>
        public Color color;

        [NonSerialized]
        private string _hex;

        /// <summary>
        /// Gets the hexadecimal string representation of the color.
        /// The value is cached after the first access to optimize performance during log generation.
        /// </summary>
        public string Hex => _hex ??= color.ToHexString();

        /// <summary>
        /// Wraps the provided content string with the defined tag in a pseudo-markup format.
        /// </summary>
        /// <param name="content">The text content to be wrapped.</param>
        /// <returns>A string formatted as [tag]content[/tag].</returns>
        public string GetTaggedText(string content) => $"[{tag}]{content}[/{tag}]";
    }

    /// <summary>
    /// Provides extension methods for collections of <see cref="CustomColorTag"/> to facilitate color processing.
    /// </summary>
    public static class CustomColorTagExtensions
    {
        /// <summary>
        /// Converts a collection of <see cref="CustomColorTag"/> into a dictionary mapping tags to their hex color strings.
        /// This dictionary is used for fast lookup during the rich text conversion process.
        /// </summary>
        /// <param name="tags">The collection of custom color tags.</param>
        /// <returns>A dictionary where the key is the tag string and the value is the hex color string.</returns>
        public static Dictionary<string, string> GetTagAndHexMap(this IEnumerable<CustomColorTag> tags)
        {
            var map = new Dictionary<string, string>();
            foreach (var t in tags)
            {
                map[t.tag] = t.Hex;
            }
            return map;
        }
    }
}