// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEditor;
using UnityEngine;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A data container that synchronizes a Unity <see cref="Object"/> with its associated 
    /// project path and persistent GUID. It provides static factory methods to resolve and encapsulate 
    /// asset metadata from various starting points (Object, Path, or GUID).
    /// </summary>
    [System.Serializable]
    internal class AssetInfo
    {
        internal static AssetInfo FromObj(Object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var path = AssetDatabase.GetAssetPath(obj);
            var guid = AssetDatabase.AssetPathToGUID(path);

            return new AssetInfo()
            {
                obj = obj,
                path = path,
                guid = guid,
                fullPath = DemoKitPaths.GetFullPath(path),
            };
        }

        internal static AssetInfo FromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var guid = AssetDatabase.AssetPathToGUID(path);
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (obj == null)
            {
                return null;
            }

            return new()
            {
                obj = obj,
                path = path,
                guid = guid,
                fullPath = DemoKitPaths.GetFullPath(path),
            };
        }

        internal static AssetInfo FromGUID(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return null;
            }

            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (obj == null)
            {
                return null;
            }

            return new()
            {
                obj = obj,
                path = path,
                guid = guid,
                fullPath = DemoKitPaths.GetFullPath(path),
            };
        }

        [SerializeField]
        internal Object obj;

        [SerializeField]
        internal string path;

        [SerializeField]
        internal string guid;

        [SerializeField]
        internal string fullPath;


        public override string ToString()
        {
            return $"name = {obj}, type = {obj.GetType()}, path = {path}, GUID = {guid}, full path = {fullPath}";
        }
    }
}