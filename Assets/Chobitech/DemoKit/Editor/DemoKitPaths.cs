// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System;
using System.IO;
using UnityEditor;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] Provides centralized path management for the DemoKit Editor tools.
    /// It handles directory resolution using persistent GUIDs to ensure reliability even if 
    /// the asset folders are moved within the Unity Project. It also provides utility functions 
    /// for path normalization and temporary directory generation.
    /// </summary>
    internal static class DemoKitPaths
    {
        internal const string demoKitRootFolderGuid = "0f1c51ba81fff4a4abbbfb3122c19483";
        internal const string demoKitRuntimeFolderGuid = "161dfbc9d04f24c4b840fb18e17d2f05";
        internal const string demoKitEditorFolderGuid = "5da7c9fda92509b41a516bf859df86aa";

        
        internal static string ConvertToSlash(string path)
            => DemoKitUtils.ConvertToSlash(path);

        internal static string GetCombinedPath(params string[] paths)
            => DemoKitUtils.GetCombinedPath(paths);
            
        internal static string GetFullPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath))
            {
                return "";
            }
            
            return DemoKitUtils.ConvertToSlash(Path.GetFullPath(assetPath));
        }

        internal static string GetRelativePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                return "";
            }
            
            return DemoKitUtils.ConvertToSlash(FileUtil.GetProjectRelativePath(fullPath));
        }


        internal static AssetInfo rootFolderInfo => DemoKitCacheData.GetAssetInfoFromGUID(demoKitRootFolderGuid);
        internal static AssetInfo editorFolderInfo => DemoKitCacheData.GetAssetInfoFromGUID(demoKitEditorFolderGuid);
        internal static AssetInfo runtimeFolderInfo => DemoKitCacheData.GetAssetInfoFromGUID(demoKitRuntimeFolderGuid);

        private static string GetAssetInfoPath(AssetInfo aInfo) => aInfo?.path ?? "";
        private static string GetAssetInfoFullPath(AssetInfo aInfo) => aInfo?.fullPath ?? "";

        internal static string RootPath => GetAssetInfoPath(rootFolderInfo);
        internal static string RootFullPath => GetAssetInfoFullPath(rootFolderInfo);

        internal static string EditorPath => GetAssetInfoPath(editorFolderInfo);
        internal static string EditorFullPath => GetAssetInfoFullPath(editorFolderInfo);
        
        internal static string RuntimePath => GetAssetInfoPath(runtimeFolderInfo);
        internal static string RuntimeFullPath => GetAssetInfoFullPath(runtimeFolderInfo);



        internal static string GenerateTempFolderFullPath() => DemoKitUtils.GetCombinedPath(Path.GetTempPath(), Guid.NewGuid().ToString("N"));


        internal static bool IsUnderAssetsFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return true;
            }

            var rPath = FileUtil.GetProjectRelativePath(path);
            return !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(rPath));
        }
    }
}