// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEditor;
using UnityEngine;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] Data container for demo configuration and deployment settings.
    /// It manages path resolution for workspace and distribution folders, tracks scene 
    /// initialization status using a <see cref="SerializableMap{K, V}"/>, and defines 
    /// the file naming conventions for generated scenes, assets, and distribution packages.
    /// </summary>
    [System.Serializable]
    internal class DemoSetupInfo
    {
        private static string GetDisplayDemoName(string demoName)
        {
            return !string.IsNullOrEmpty(demoName) ? demoName : DemoKitUtils.DefaultValues.DemoName;
        }


        [SerializeField]
        internal string demoName;
        internal string DisplayDemoName => GetDisplayDemoName(demoName);
        

        [SerializeField]
        internal string workspaceFolderPath;

        [SerializeField]
        internal bool overwriteFilesInWorkspaceHolder = false;
        
        [SerializeField]
        internal bool isAddEventSystemForDemoCreation = true;


        [SerializeField]
        internal string distributionFolderPath;

        [SerializeField]
        internal bool overwriteFilesInDistributionFolder = true;

        [SerializeField]
        internal bool isRemoveEventSystemBeforeDistribution = true;

        // TODO: valid folder path is under Assets/
        //internal string RelativeWorkspaceFolderPath => DemoKitPaths.GetRelativePath(workspaceFolderPath);

        internal string RelativeWorkspaceFolderPath
        {
            get
            {
                if (!DemoKitPaths.IsUnderAssetsFolder(workspaceFolderPath))
                {
                    return "";
                }

                return DemoKitPaths.GetRelativePath(workspaceFolderPath);
            }
        }

        internal bool IsValidWorkspacePath => !string.IsNullOrEmpty(RelativeWorkspaceFolderPath);


        internal string RelativeDistributionFolderPath => DemoKitPaths.GetRelativePath(distributionFolderPath);

        internal string DemoMainSceneFileName => DemoKitUtils.DefaultValues.DemoMainSceneFileName;
        internal string GlobalDemoInfoFileName => DemoKitUtils.DefaultValues.GlobalDemoInfoFileName;

        internal string FullWorkspaceDemoMainScenePath => DemoKitPaths.GetCombinedPath(
            workspaceFolderPath,
            DemoMainSceneFileName
        );

        internal string RelativeWorkspaceDemoMainScenePath => DemoKitPaths.GetCombinedPath(
            RelativeWorkspaceFolderPath,
            DemoMainSceneFileName
        );

        internal string FullWorkspaceGlobalDemoInfoPath => DemoKitPaths.GetCombinedPath(
            workspaceFolderPath,
            GlobalDemoInfoFileName
        );
        internal string RelativeWorkspaceGlobalDemoInfoPath => DemoKitPaths.GetCombinedPath(
            RelativeWorkspaceFolderPath,
            GlobalDemoInfoFileName
        );

        [SerializeField]
        internal SceneAsset demoMainScene;

        [SerializeField]
        internal GlobalDemoInfo globalDemoInfo;


        internal bool IsRequiredAssetsSet => demoMainScene != null && globalDemoInfo != null;
        

        [SerializeField]
        private SerializableMap<string, SceneInitializedStatus> sceneInitializedStatusMap = new();

        internal SceneInitializedStatus GetSceneInitializedStatus(string sceneGuid)
        {
            if (sceneInitializedStatusMap.TryGetValue(sceneGuid, out var res))
            {
                return res;
            }
            return SceneInitializedStatus.SceneNotExists;
        }

        internal void SetSceneInitializedStatus(string sceneGuid, SceneInitializedStatus status)
            => sceneInitializedStatusMap[sceneGuid] = status;

        internal string DistributionAssetsZipFileName => DemoKitUtils.DefaultValues.DistributionAssetsZipFileName;
        internal string DistributionAssetsZipFileFullPath => DemoKitPaths.GetCombinedPath(distributionFolderPath, DistributionAssetsZipFileName);


        internal string DistributionDemoInitializerCodeFileName => DemoKitUtils.DefaultValues.InitializerCodeFileNameSuffix;
        internal string DistributionDemoInitializerCodeFullPath => DemoKitPaths.GetCombinedPath(distributionFolderPath, DistributionDemoInitializerCodeFileName);

        internal string DistributionInternalUnityPackageFullPath => DemoKitPaths.GetCombinedPath(distributionFolderPath, DemoKitUtils.DefaultValues.DistributionInternalCodePackageName);


        internal void Save()
        {
            DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                if (sInfo == this)
                {
                    DemoSetupSettings.Save();
                }
            });
        }
    }
}