// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A singleton data store that caches <see cref="AssetInfo"/> to optimize asset resolution in the Editor.
    /// By using <see cref="ScriptableSingleton{T}"/>, it maintains a persistent cache of Object-Path-GUID mappings,
    /// reducing frequent and expensive calls to the <see cref="AssetDatabase"/> and ensuring reliable asset tracking
    /// throughout the DemoKit's editor-side operations.
    /// </summary>
    internal class DemoKitCacheData : ScriptableSingleton<DemoKitCacheData>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<AssetInfo> assetInfoList = new();

        private Dictionary<string, AssetInfo> _pathMap;
        private Dictionary<string, AssetInfo> pathMap => _pathMap ??= new();

        private Dictionary<string, AssetInfo> _guidMap;
        private Dictionary<string, AssetInfo> guidMap => _guidMap ??= new();


        /*
        private static bool TryGetAssetInfo(System.Func<AssetInfo, bool> predicate, out AssetInfo aInfo)
        {
            aInfo = null;
            foreach (var a in instance.assetInfoList)
            {
                if (predicate(a))
                {
                    aInfo = a;
                    break;
                }
            }
            return aInfo != null;
        }
        */

        private static bool TryGetAssetInfo(
            //System.Func<AssetInfo, bool> predicate,
            System.Func<AssetInfo> getter,
            out AssetInfo aInfo
        )
        {
            aInfo = getter();
            return aInfo != null;
        }

        private static AssetInfo GetAssetInfo(
            System.Func<bool> checkEnabled,
            
            //System.Func<AssetInfo, bool> predicate,
            //System.Func<bool> predicate,
            System.Func<AssetInfo> getter,

            System.Func<AssetInfo> generator
        )
        {
            if (!checkEnabled())
            {
                return null;
            }

            if (!TryGetAssetInfo(getter, out var aInfo))
            {
                aInfo = generator();
                if (aInfo != null)
                {
                    instance.assetInfoList.Add(aInfo);
                    instance.pathMap[aInfo.path] = aInfo;
                    instance.guidMap[aInfo.guid] = aInfo;
                }
            }

            return aInfo;
        }

        private static void Remove(AssetInfo aInfo)
        {
            if (aInfo == null)
            {
                return;
            }

            instance.pathMap.Remove(aInfo.path);
            instance.guidMap.Remove(aInfo.guid);
            instance.assetInfoList.Remove(aInfo);
        }

        private static void Remove(string path)
            => Remove(GetAssetInfoFromPath(path));

        internal static AssetInfo GetAssetInfoFromObject(Object o)
            => GetAssetInfo(
                () => o != null,
                () => instance.assetInfoList.FirstOrDefault(aInfo => aInfo.obj == o),
                () => AssetInfo.FromObj(o)
            );
        

        internal static AssetInfo GetAssetInfoFromGUID(string guid)
            => GetAssetInfo(
                () => !string.IsNullOrEmpty(guid),
                () =>
                {
                    _ = instance.guidMap.TryGetValue(guid, out var res);
                    return res;
                },
                () => AssetInfo.FromGUID(guid)
            );
        
        internal static AssetInfo GetAssetInfoFromPath(string path)
            => GetAssetInfo(
                () => !string.IsNullOrEmpty(path),
                () =>
                {
                    _ = instance.pathMap.TryGetValue(path, out var res);
                    return res;
                },
                () => AssetInfo.FromPath(path)
            );


        internal static string GetAssetGUID(Object o)
            => GetAssetInfoFromObject(o)?.guid;

        internal static string GetAssetGUID(string path)
            => GetAssetInfoFromPath(path)?.guid;
        
        internal static string GetAssetPath(Object o)
            => GetAssetInfoFromObject(o)?.path;
        
        internal static string GetAssetPath(string guid)
            => GetAssetInfoFromGUID(guid)?.path;
        
        internal static T GetAssetObjectFromGUID<T>(string guid) where T : Object
            => GetAssetInfoFromGUID(guid)?.obj as T;
        
        internal static T GetAssetObjectFromPath<T>(string path) where T : Object
            => GetAssetInfoFromPath(path)?.obj as T;



        internal static AssetInfo AddObject(Object o)
        {
            var aInfo = AssetInfo.FromObj(o);
            if (aInfo != null)
            {
                instance.assetInfoList.Add(aInfo);
            }
            return aInfo;
        }

        internal static void RemoveAssetInfoWithObject(Object o)
        {
            instance.assetInfoList.RemoveAll(aInfo =>
            {
                return aInfo.obj == o;
            });
        }

        internal static void RemoveAssetInfoWithGUID(string guid)
        {
            instance.assetInfoList.RemoveAll(aInfo =>
            {
                return aInfo.guid == guid;
            });
        }

        internal static void RemoveAssetInfoWithPath(string path)
        {
            instance.assetInfoList.RemoveAll(aInfo =>
            {
                return aInfo.path == path;
            });
        }

        internal static void RemoveAssetInfo(AssetInfo assetInfo)
        {
            instance.assetInfoList.Remove(assetInfo);
        }


        internal static void RefreshAllAssetInfo()
        {
            instance.assetInfoList = new();
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            foreach (var aInfo in assetInfoList)
            {
                pathMap[aInfo.path] = aInfo;
                guidMap[aInfo.guid] = aInfo;
            }
        }



        private class DkCacheDataRefresher : AssetPostprocessor
        {
            private static void DeleteAssetInfo(string[] removedPaths)
            {
                foreach (var path in removedPaths)
                {
                    Remove(path);
                }
            }

            private static void UpdateAssetInfo(List<string> paths)
            {
                var guids = paths
                    .Select(p => AssetDatabase.AssetPathToGUID(p))
                    .ToArray();
                
                var count = instance.assetInfoList.Count;

                for (var i = 0; i < count; i++)
                {
                    var aInfo = instance.assetInfoList[i];
                    for (var k = 0; k < guids.Length; k++)
                    {
                        var path = paths[k];
                        if (aInfo.guid == guids[k])
                        {
                            if (path != aInfo.path)
                            {
                                instance.pathMap.Remove(aInfo.path); 
                                aInfo.path = path;
                                aInfo.fullPath = DemoKitPaths.GetFullPath(path);
                                instance.pathMap[path] = aInfo;
                            }
                            break;
                        }
                    }
                }
            }

            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                DeleteAssetInfo(deletedAssets);

                var paths = new List<string>();
                paths.AddRange(importedAssets);
                paths.AddRange(movedAssets);

                UpdateAssetInfo(paths);

           }
        }
    }
}