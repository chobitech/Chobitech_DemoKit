// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Chobitech.DemoKit.DevInternal
{
    public static class DevInternalUtils
    {
                public static string GetAssetContent(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return null;
            }

            var fullPath = DemoKitUtils.ConvertToSlash(Path.GetFullPath(relativePath));

            if (!string.IsNullOrEmpty(fullPath) && File.Exists(fullPath))
            {
                try
                {
                    return File.ReadAllText(fullPath);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            return null;
        }

        public static string GetAssetContentByGuid(string guid)
        {
            return GetAssetContent(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}
#endif
