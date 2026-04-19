// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A concrete implementation of <see cref="BaseStepLayout"/> that manages the creation and 
    /// configuration of essential demo assets. This includes copying the master demo scene, 
    /// generating a <see cref="GlobalDemoInfo"/> ScriptableObject with preset color tags, 
    /// and optionally injecting an EventSystem into the target scene. It automatically 
    /// assigns these assets to <see cref="DemoSetupSettings"/> upon creation.
    /// </summary>
    internal class SetupRequiredAssetsLayout : BaseStepLayout
    {
        private static readonly GUIContent overwriteExistingFilesLabel = new("Overwrite existing files", "[NOTICE] The default value of this flag is false. It is not saved or restored in order to prevent overwriting existing files.");


        private bool _isCopyOk = false;

        private SceneAsset _scene;
        private GlobalDemoInfo _gdi;


        internal override string Title => "Setup Required Assets";

        internal override string Description => "Press the button below to copy required assets to the selected workspace.";


        private bool CopyDemoMainScene()
        {
            var isDemoMainSceneExists = DemoKitEditorUtils.IsGuidExists(DemoKitUtils.PresetGUID.DemoMainScene);
            if (!isDemoMainSceneExists)
            {
                DemoKitLog.Error($"The required scene file {DemoKitUtils.DefaultValues.DemoMainSceneFileName} is not found.");
                return false;
            }

            return DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                if (string.IsNullOrEmpty(sInfo.demoName))
                {
                    DemoKitLog.Warning($"DemoName is empty. DemoKit use default name: {DemoKitUtils.DefaultValues.DemoName}.");
                }

                var demoSceneName = Path.GetFileNameWithoutExtension(sInfo.DemoMainSceneFileName);

                var destPath = DemoKitPaths.GetCombinedPath(
                    sInfo.RelativeWorkspaceFolderPath,
                    sInfo.DemoMainSceneFileName
                );

                SceneAsset targetScene = DemoKitCacheData.GetAssetObjectFromPath<SceneAsset>(destPath);

                if (targetScene != null)
                {
                    DemoKitLog.Info($"{demoSceneName} already exists, so it is used as the scene of {sInfo.demoName}");
                }
                else
                {
                    targetScene = DemoKitEditorUtils.CopyAssetWithGuid<SceneAsset>(DemoKitUtils.PresetGUID.DemoMainScene, destPath);
                    DemoKitLog.Info($"The preset scene {DemoKitUtils.DefaultValues.DemoMainSceneName} was copied to {destPath}.");
                }

                if (targetScene != null)
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    sInfo.demoMainScene = targetScene;
                    sInfo.Save();

                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        private bool CopyGlobalDemoInfo()
            => DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                var gdiName = Path.GetFileNameWithoutExtension(sInfo.GlobalDemoInfoFileName);
                
                var destPath = sInfo.RelativeWorkspaceGlobalDemoInfoPath;

                var gdi = DemoKitCacheData.GetAssetObjectFromPath<GlobalDemoInfo>(destPath);

                if (gdi != null)
                {
                    DemoKitLog.Info($"{gdiName} already exists, so it was set as {nameof(GlobalDemoInfo)} of {sInfo.demoName}");
                }
                else
                {
                    gdi = GenerateNewGlobalDemoInfo();
                    if (gdi != null)
                    {
                        gdi.title = sInfo.DisplayDemoName;
                        DemoKitLog.Info($"New {DemoKitUtils.DefaultValues.GlobalDemoInfoName} asset was generated: {destPath}.");
                    }
                    else
                    {
                        DemoKitLog.Error($"Generating GlobalDemoInfo failed.");
                        return false;
                    }
                }

                if (gdi != null)
                {
                    sInfo.globalDemoInfo = gdi;
                    sInfo.Save();

                    return true;
                }
                else
                {
                    return false;
                }
            });


        private GlobalDemoInfo GenerateNewGlobalDemoInfo()
            => DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                var gdiFileName = sInfo.GlobalDemoInfoFileName;

                var destPath = DemoKitPaths.GetCombinedPath(
                    sInfo.RelativeWorkspaceFolderPath,
                    gdiFileName
                );

                try
                {
                    var gdiInstance = ScriptableObject.CreateInstance<GlobalDemoInfo>();
                    gdiInstance.title = sInfo.DisplayDemoName;

                    var presetNoticeColorTag = DemoKitEditorUtils.LoadObjectFromGUID<CustomColorTag>(DemoKitUtils.PresetGUID.NoticeColorTag);
                    var presetErrorColorTag = DemoKitEditorUtils.LoadObjectFromGUID<CustomColorTag>(DemoKitUtils.PresetGUID.ErrorColorTag);
                    var presetWarningColorTag = DemoKitEditorUtils.LoadObjectFromGUID<CustomColorTag>(DemoKitUtils.PresetGUID.WarningColorTag);

                    gdiInstance.customColorTags = new CustomColorTag[] {
                    presetNoticeColorTag,
                    presetWarningColorTag,
                    presetErrorColorTag
                };

                    AssetDatabase.CreateAsset(gdiInstance, destPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    return gdiInstance;
                }
                catch (Exception ex)
                {
                    DemoKitLog.Exception(ex);
                    return null;
                }
            });

        
        private void AddDemoNameObject(Scene scene, DemoSetupInfo sInfo)
        {
            var objName = $"--- {sInfo.demoName} ---";
            
            var nameObj = scene.GetRootGameObjects()
                .FirstOrDefault(obj => obj.name == objName);
            
            if (nameObj == null)
            {
                nameObj = new GameObject(objName);
                SceneManager.MoveGameObjectToScene(nameObj, scene);
                nameObj.transform.SetAsFirstSibling();
                nameObj.tag = "EditorOnly";
            }
        }
        

        private void ExecCopyFiles(DemoSetupInfo sInfo)
        {
            var mainSceneCopyOk = CopyDemoMainScene();
            var globalDemoInfoCopyOk = CopyGlobalDemoInfo();

            if (mainSceneCopyOk && globalDemoInfoCopyOk)
            {
                var autoAddEventSystem = sInfo.isAddEventSystemForDemoCreation;

                DemoKitSceneUtils.EditScene(
                    sInfo.RelativeWorkspaceDemoMainScenePath,
                    scene =>
                    {
                        // add DemoName empty object
                        AddDemoNameObject(scene, sInfo);

                        // apply GlobalDemoInfo to DemoOrchestrator
                        DemoKitEditorUtils.ApplyGlobalDemoInfoToDemoOrchestrator(scene, sInfo.globalDemoInfo);

                        // add EventSystem or dummy
                        DemoKitSceneUtils.GenerateEventSystem(scene);
                    }
                );
            }
        }

        protected override bool? GetStepChecked()
        {
            return DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                return sInfo.IsValidWorkspacePath && (sInfo.demoMainScene != null) && (sInfo.globalDemoInfo != null);
            });
        }

        protected override bool InnerStepLayout()
            => DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                _isCopyOk = sInfo.IsValidWorkspacePath;

                var needsRepaint = false;

                DemoKitGUI.SmallSpace();

                var isAddEventSystem = sInfo.isAddEventSystemForDemoCreation;

                DemoKitGUI.EnabledSwitcher(_isCopyOk, b =>
                {
                    needsRepaint |= DemoKitGUI.CheckBox(
                        $"Auto-add EventSystem",
                        isAddEventSystem,
                        b =>
                        {
                            sInfo.isAddEventSystemForDemoCreation = b;
                            sInfo.Save();
                        }
                    );
                });

                DemoKitGUI.SmallSpace();

                DemoKitGUI.InfoBox($"Existing files will not be overwritten and will be used as-is.");

                DemoKitGUI.SmallSpace();

                DemoKitGUI.Button(
                    "Setup Required Assets",
                    () => ExecCopyFiles(sInfo),
                    _isCopyOk
                );

                DemoKitGUI.SmallSpace();

                DemoKitGUI.Label("Setup Assets (Auto-assigned):");


                _scene = sInfo.demoMainScene;

                _ = DemoKitGUI.WithObjectChangeCheck(
                    "Demo Main Scene",
                    _scene,
                    s =>
                    {
                        sInfo.demoMainScene = s;
                        sInfo.Save();
                        needsRepaint |= s != _scene;

                        Debug.Log($"DMS changed");

                        // TODO: open and set GlobalDemoInfo if GDI is not null
                    }
                );


                _gdi = sInfo.globalDemoInfo;
                _ = DemoKitGUI.WithObjectChangeCheck(
                    "Global Demo Info",
                    _gdi,
                    info =>
                    {
                        sInfo.globalDemoInfo = info;
                        sInfo.Save();
                        needsRepaint |= info != _gdi;

                        // TODO: open and set to DemoMainScene if DMS is not null
                    }
                );

                if (!sInfo.IsRequiredAssetsSet)
                {
                    DemoKitGUI.ErrorBox($"{DemoKitUtils.DefaultValues.DemoMainSceneName} and/or {DemoKitUtils.DefaultValues.GlobalDemoInfoName} are not set.");
                }

                return needsRepaint;
            }
        );
    }
}
