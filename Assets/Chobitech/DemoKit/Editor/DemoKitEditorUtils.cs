// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A utility class providing helper methods for Unity Editor operations.
    /// It simplifies asset management by wrapping <see cref="AssetDatabase"/> operations, 
    /// manages asset resolution through cached GUIDs and paths via <see cref="DemoKitCacheData"/>, 
    /// and provides UI utilities for folder selection and JSON data mapping.
    /// </summary>
    internal static class DemoKitEditorUtils
    {
        internal static bool IsApplicationPlaying => Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode;

        internal static string GetAssetGUID(Object o)
            => DemoKitCacheData.GetAssetGUID(o);
        
        internal static string GetAssetGUID(string path)
            => DemoKitCacheData.GetAssetGUID(path);
        
        
        internal static string GetAssetPath(Object o)
            => DemoKitCacheData.GetAssetPath(o);
        
        internal static string GetAssetPath(string guid)
            => DemoKitCacheData.GetAssetPath(guid);

        
        internal static T GetAssetObjectFromGUID<T>(string guid) where T : Object
            => DemoKitCacheData.GetAssetObjectFromGUID<T>(guid);
        
        internal static T GetAssetObjectFromPath<T>(string path) where T : Object
            => DemoKitCacheData.GetAssetObjectFromPath<T>(path);

        internal static string GetSceneGUID(Scene scene)
        {
            return DemoKitCacheData.GetAssetGUID(scene.path);
        }

        internal static bool IsGuidExists(string guid)
        {
            return DemoKitCacheData.GetAssetInfoFromGUID(guid) != null;
        }

        internal static bool IsPathExists(string path)
        {
            return DemoKitCacheData.GetAssetInfoFromPath(path) != null;
        }

        internal static T CopyAssetWithGuid<T>(string srcGuid, string destPath) where T : UnityEngine.Object
        {
            var srcPath = GetAssetPath(srcGuid);

            if (string.IsNullOrEmpty(srcPath))
            {
                return default;
            }

            try
            {
                if (AssetDatabase.CopyAsset(srcPath, destPath))
                {
                    return AssetDatabase.LoadAssetAtPath<T>(destPath);
                }
            }
            catch (System.Exception ex)
            {
                DemoKitLog.Exception(ex);
            }

            return default;
        }
        

        internal static string SelectFolderInAssets(string title, string initFolderPath = null, bool returnInitFolderPathIfCanceled = true)
        {
            var path = EditorUtility.OpenFolderPanel(title, !string.IsNullOrEmpty(initFolderPath) ? initFolderPath : "Assets", "");

            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }

            return returnInitFolderPathIfCanceled ? initFolderPath : null;
        }


        internal static T LoadObjectFromGUID<T>(string guid) where T : UnityEngine.Object
             => DemoKitCacheData.GetAssetObjectFromGUID<T>(guid);

        internal static Dictionary<K, V> MapFromJson<K, V>(string json)
        {
            json = !string.IsNullOrEmpty(json) ? json : "{}";
            return JsonUtility.FromJson<Dictionary<K, V>>(json);
        }

        internal static void SetEntriesFromJson<K, V>(Dictionary<K, V> srcMap, string json, bool clearSrcEntries = true)
        {
            var map = MapFromJson<K, V>(json);
            if (clearSrcEntries)
            {   
                srcMap.Clear();
            }

            foreach (var m in map)
            {
                srcMap[m.Key] = m.Value;
            }
        }

        internal static void ApplyGlobalDemoInfoToDemoOrchestrator(Scene scene, GlobalDemoInfo globalDemoInfo)
        {
            var orchestrator = scene.GetRootGameObjects()
                .Select(obj => obj.GetComponent<DemoOrchestrator>())
                .FirstOrDefault(o => o != null);
            
            if (orchestrator != null)
            {
                orchestrator.globalDemoInfo = globalDemoInfo;
            }
        }
    }
}