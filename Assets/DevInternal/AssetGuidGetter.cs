// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


#if UNITY_EDITOR
using UnityEditor;

namespace Chobitech.DemoKit.DevInternal
{
    internal class AssetGuidTool
    {
        [MenuItem("Assets/Copy Asset GUID", false, 100)]
        private static void CopySelectedAssetGuid()
        {
            var selectedGuids = Selection.assetGUIDs;

            if (selectedGuids.Length != 1)
            {
                DemoKitLog.Error($"GUID copy error: Multiple objects are selected. Select single object.");
                return;
            }

            var guid = selectedGuids[0];

            EditorGUIUtility.systemCopyBuffer = guid;
            DemoKitLog.Info($"GUID Copied: {guid}");
        }
    }
}
#endif
