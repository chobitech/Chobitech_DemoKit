// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Chobitech.DemoKit.DevInternal
{
    internal class DemoKitFileSync : AssetPostprocessor
    {
        private static readonly string[] copyToRootFileGuid = new string[]
        {
            "fefad1c66ebf8ba4aa83b1bf39395c06",     // CHANGELOG.md
            "a427f77b2c2914a47ba3b7a61c00ff84",     // LICENSE.md
            "6c0a6019cd9a2564496ebaf62f7316fc",     // README.md
        };

        private static void CopyFilesToRoot(string[] updatedAssetPaths)
        {
            var filePathList = copyToRootFileGuid
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .ToArray();
            
            var projRoot = Directory.GetParent(Application.dataPath).FullName;
            

            foreach (var aPath in updatedAssetPaths)
            {
                foreach (var p in filePathList)
                {
                    if (aPath == p)
                    {
                        var fName = Path.GetFileName(p);
                        var destFullPath = DemoKitUtils.GetCombinedPath(projRoot, fName);
                        var srcFullPath = DemoKitUtils.GetCombinedPath(projRoot, p);
                        
                        try
                        {
                            File.Copy(srcFullPath, destFullPath, true);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }            
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            CopyFilesToRoot(importedAssets);
        }
    }
}
#endif
