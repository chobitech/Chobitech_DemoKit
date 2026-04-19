// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEditor;

namespace Chobitech.DemoKit.Editor
{
    internal class TMPImportedChecker : AssetPostprocessor
    {
        internal static bool CheckIsImported()
        {
            var sdfPath = DemoKitCacheData.GetAssetPath(DemoKitUtils.PresetGUID.TMPEssentialResourceCheckGUID);

            if (!string.IsNullOrEmpty(sdfPath))
            {
                if (AssetDatabase.GetMainAssetTypeAtPath(sdfPath) != null)
                {
                    return true;
                }
            }

            var guids = AssetDatabase.FindAssets("t:TMP_Settings");
            return guids.Length > 0;
        }


        private static bool? _innerIsImported = null;


        internal static bool IsImported => _innerIsImported ??= CheckIsImported();

        internal static void Reset()
        {
            _innerIsImported = null;
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            Reset();
        }
    }
}
