// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.IO;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// Provides utility constants and shared properties for the DemoKit system.
    /// </summary>
    public static partial class DemoKitUtils
    {
        public const string ReverseFqdn = "com.chobitech.unity.demo-kit";
        public const string PackageName = "Chobitech.DemoKit";
        public const string WebsiteUrl = "https://chobitech.com/";
        public const string GitHubUrl = "https://github.com/chobitech";
        public const string DemoKitDocumentUrl = "https://chobitech.github.io/Chobitech_DemoKit_Docs/";
        public const string AssetStoreUrl = "https://assetstore.unity.com/publishers/135416";

        /// <summary>
        /// Contains default strings and file names used during the demo generation process.
        /// </summary>
        public static class DefaultValues
        {
            public const string DemoName = "MyDemoKitDemo";

            public const string DemoMainSceneName = "DemoMainScene";
            public const string DemoMainSceneFileName = DemoMainSceneName + ".unity";

            public const string GlobalDemoInfoName = "GlobalDemoInfo";
            public const string GlobalDemoInfoFileName = GlobalDemoInfoName + ".asset";
            
            public const string DistributionInternalCodePackageNameWithoutExtension = "DemoKit_Internal";
            public const string DistributionInternalCodePackageName = DistributionInternalCodePackageNameWithoutExtension + ".unitypackage";
            public const string DistributionAssetsZipFileName = "DemoAssets.zip";

            public const string InitializerCodeFileNameSuffix = "DemoAutoInitializer.cs";
        }

        /// <summary>
        /// Contains constant GUID strings for identifying specific assets and folders within the project.
        /// </summary>
        public static class PresetGUID
        {
            public const string DemoMainScene = "0fcc90d5cfd427c4e8203cdb5d1fca26";

            public const string NoticeColorTag = "7dca9c7f9edcd69418cd5f41bf6610f5";
            public const string ErrorColorTag = "1ac5878a5aa76634eac06a89d582aa04";
            public const string WarningColorTag = "bc4bd204fa4a9824caaecf7516f11767";

            public const string DemoOrchestratorGuid = "712617d2661f98c4cbc44bc48c0dc3ce";

            public const string InternalFolder = "9bae67bcfaf7a6242b135b0aa4bdd0ce";

            public const string EditorFolder = "5da7c9fda92509b41a516bf859df86aa";

            public const string TMPEssentialResourceCheckGUID = "8f586378b4e144a9851e7b34d9b748ee";
        }

        public static string ConvertToSlash(string path)
        {
            if (path == null)
            {
                return "";
            }

            return path.Replace("\\", "/");
        }

        public static string GetCombinedPath(params string[] paths)
        {
            return ConvertToSlash(Path.Combine(paths));
        }
    }
}