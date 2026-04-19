// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


#if UNITY_EDITOR
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace Chobitech.DemoKit.DevInternal
{

    // https://img.shields.io/badge/version-1.0.0-orange.svg

    internal class DemoKitVersionSynchronizer : AssetPostprocessor
    {
        private static readonly Regex versionFormatRegex = new(@"https://img\.shields\.io/badge/version-(?<version>\d[\d.]+\d)", RegexOptions.IgnoreCase);
        private static readonly Regex jsonVersionRegex = new(@"""version"":\s+""(?<version>\d[\d.]+\d)""", RegexOptions.IgnoreCase);

        private static readonly Regex guidRegex = new("^[0-9a-f]+$", RegexOptions.IgnoreCase);

        private const string packageJsonGuid = "24b3843eebc4ed44490c90782119e085";

        private static string GetCurrentVersion()
        {
            var json = DevInternalUtils.GetAssetContentByGuid(packageJsonGuid);
            var m = jsonVersionRegex.Match(json);

            if (m == null || !m.Success)
            {
                return null;
            }

            return m.Groups["version"].Value;
        }

        private static readonly string[] replaceTargetFileGuidAndPaths = new string[]
        {
            "6c0a6019cd9a2564496ebaf62f7316fc",     // README.md
            "Assets/../Documents/index.md",
            "Assets/../Documents/getting-started.md",
        };

        [MenuItem("Chobitech/Internal/Version Update")]
        private static void UpdateVersion()
        {
            InnerUpdateVersion();
        }

        private static void InnerUpdateVersion(IEnumerable<string> excepts = null)
        {
            excepts ??= new List<string>();

            var targetPaths = replaceTargetFileGuidAndPaths
                .Select(p =>
                {
                    var rPath = p;

                    if (guidRegex.IsMatch(p))
                    {
                        rPath = AssetDatabase.GUIDToAssetPath(p);
                    }

                    return rPath;
                })
                .Where(p => !string.IsNullOrEmpty(p))
                .ToArray();

            if (targetPaths.Count() <= 0)
            {
                return;
            }

            var curVersion = GetCurrentVersion();
            
            foreach(var rPath in targetPaths)
            {
                try
                {
                    var content = DevInternalUtils.GetAssetContent(rPath);
                    if (string.IsNullOrEmpty(content))
                    {
                        continue;
                    }

                    var match = versionFormatRegex.Match(content);
                    if (match == null || !match.Success)
                    {
                        continue;
                    }

                    var ver = match.Groups["version"].Value;
                    if (ver == curVersion)
                    {
                        continue;
                    }

                    content = versionFormatRegex.Replace(
                        content,
                        m =>
                        {
                            return m.Value.Replace(m.Groups["version"].Value, curVersion);
                        }
                    );

                    File.WriteAllText(Path.GetFullPath(rPath), content);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            var pkgJsonPath = AssetDatabase.GUIDToAssetPath(packageJsonGuid);

            if (string.IsNullOrEmpty(pkgJsonPath))
            {
                return;
            }

            foreach (var aPath in importedAssets)
            {
                if (aPath == pkgJsonPath)
                {
                    InnerUpdateVersion();
                    return;
                }
            }
        }
    }
}
#endif
