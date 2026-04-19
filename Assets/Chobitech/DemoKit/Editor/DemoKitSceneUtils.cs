// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] Provides utility methods for scene manipulation and validation within the DemoKit.
    /// It handles automatic scene editing (Open/Action/Save/Close), EventSystem detection 
    /// and injection, and ensures that the <see cref="DemoOrchestrator"/> is correctly 
    /// configured with the designated <see cref="GlobalDemoInfo"/> upon scene initialization.
    /// </summary>
    [InitializeOnLoad]
    internal static class DemoKitSceneUtils
    {
        internal static AssetInfo DemoMainSceneAssetInfo
            => DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo => DemoKitCacheData.GetAssetInfoFromObject(sInfo.demoMainScene));

        private const string eventSystemDummyObjectName = "[Add EventSystem on Your Environment]";


        internal static void EditScene(string scenePath, UnityAction<Scene> action)
        {
            var scene = EditorSceneManager.GetSceneByPath(scenePath);
            var isAlreadyOpened = scene.IsValid();
            if (!isAlreadyOpened)
            {
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }

            try
            {
                action?.Invoke(scene);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
            finally
            {
                if (!isAlreadyOpened)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        internal static bool IsDemoMainScene(Scene scene)
        {
            var aInfo = DemoKitCacheData.GetAssetInfoFromPath(scene.path);
            return (aInfo != null) && aInfo.guid == DemoMainSceneAssetInfo?.guid;
        }

        internal static List<GameObject> GetAllEventSystemHolder(Scene scene)
        {
            return scene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<EventSystem>(true))
                .Select(es => es.gameObject)
                .Distinct()
                .ToList();
        }

        internal static void RemoveEventSystemFromScene(Scene scene)
        {
            var allEsHolder = GetAllEventSystemHolder(scene);
            foreach (var holder in allEsHolder)
            {
                GameObject.DestroyImmediate(holder);
            }
        }

        private static T RunWithCurrentScene<T>(System.Func<Scene, T> proc)
        {
            var curScene = EditorSceneManager.GetActiveScene();
            return proc(curScene);
        }

        private static void RunWithCurrentScene(UnityAction<Scene> action)
            => RunWithCurrentScene<bool>(
                scene =>
                {
                    action(scene);
                    return false;
                }
            );


        private static T RunWithCurrentSceneIsDemoMainScene<T>(System.Func<Scene, AssetInfo, T> proc)
            => RunWithCurrentScene<T>(
                scene =>
                {
                    if (IsDemoMainScene(scene))
                    {
                        return proc(scene, DemoMainSceneAssetInfo);
                    }
                    return default;
                }
            );
        
        private static void RunWithCurrentSceneIsDemoMainScene(UnityAction<Scene, AssetInfo> action)
            => RunWithCurrentSceneIsDemoMainScene<bool>(
                (scene, aInfo) =>
                {
                    action(scene, aInfo);
                    return false;
                }
            );

        internal static List<GameObject> GetGameObjectsInScene(Scene scene, System.Func<GameObject, bool> predicate)
        {
            if (scene == null)
            {
                return new();
            }

            var objs = scene.GetRootGameObjects();

            if (predicate == null)
            {
                return objs.ToList();
            }

            return objs.Where(predicate).ToList();
        }


        internal static bool IsCurrentSceneIsDemoMainScene
            => RunWithCurrentSceneIsDemoMainScene((_, _) => true);

        private static List<GameObject> GetDummyEventSystemObjectInScene(Scene scene)
        {
            return GetGameObjectsInScene(scene, o => o.name == eventSystemDummyObjectName);
        }

        internal static void RemoveDummyEventSystemObjects(Scene scene)
        {
            foreach (var dummy in GetDummyEventSystemObjectInScene(scene))
            {
                GameObject.DestroyImmediate(dummy);
            }
        }

        private static bool IsDummyObjectExistsInScene(Scene scene)
            => GetDummyEventSystemObjectInScene(scene).Count > 0;

        
        private static bool IsEvenSystemExistsInScene(Scene scene)
            => GetGameObjectsInScene(scene, o => o.TryGetComponent<EventSystem>(out var _ )).Count > 0;
        
        internal static void GenerateDummyObj(Scene scene)
        {
            if (IsDummyObjectExistsInScene(scene))
            {
                return;
            }

            var go = new GameObject(eventSystemDummyObjectName)
            {
                tag = "EditorOnly"
            };

            SceneManager.MoveGameObjectToScene(go, scene);
        }

        internal static void GenerateEventSystem(Scene scene)
        {
            var isEsExists = scene.GetRootGameObjects()
                .Any(obj => obj.GetComponentInChildren<EventSystem>(true) != null);
            
            if (isEsExists)
            {
                return;
            }
            
            var go = new GameObject("EventSystem");
            SceneManager.MoveGameObjectToScene(go, scene);

            go.AddComponent<EventSystem>();

            var inputSystemType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");

            if (inputSystemType != null)
            {
                go.AddComponent(inputSystemType);
            }
            else
            {
                go.AddComponent<StandaloneInputModule>();
            }
        }

        internal static void ShowCreateEventSystemDialog()
            => RunWithCurrentSceneIsDemoMainScene((scene, aInfo) =>
            {
                if (IsEvenSystemExistsInScene(scene))
                {
                    return;
                }

                if (!DemoKitUserSettings.IsDoNotAskAddEventSystem(aInfo.guid))
                {

                    var result = EditorUtility.DisplayDialogComplex(
                        $"{DemoKitUtils.PackageName} Setup",
                        $"EventSystem is missing in the scene \"{scene.name}\". Would you like to add it automatically for creating your demo?",
                        "Yes",
                        "No",
                        "No (Don't ask again)"
                    );

                    if (result != 0)
                    {
                        GenerateDummyObj(scene);
                    }

                    switch (result)
                    {
                        case 0:
                            GenerateEventSystem(scene);
                            RemoveDummyEventSystemObjects(scene);
                            return;

                        case 2:
                            DemoKitUserSettings.SetIsDoNotAskAddEventSystem(aInfo.guid, true);
                            DemoKitUserSettings.Save();
                            break;

                        default:
                            break;
                    }
                }

                DemoKitLog.Warning($"EventSystem is missing in {aInfo.obj.name}. Add <b>EventSystem</b> object to {aInfo.obj.name} to control the UI.");
        });


        internal static GlobalDemoInfo GetCurrentSetGlobalDemoInfo(Scene scene)
        {
            var orchestrator = scene.GetRootGameObjects()
                .Select(obj => obj.GetComponent<DemoOrchestrator>())
                .FirstOrDefault(o => o != null);

            return (orchestrator != null)
                ? orchestrator.globalDemoInfo
                : null;
        }


        internal static void CheckSceneSettings(Scene scene)
            => DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                var orchestrator = GetCurrentSetGlobalDemoInfo(scene);
                if (orchestrator == null)
                {
                    DemoKitLog.Warning($"The {nameof(DemoOrchestrator)} object is missing.");
                    return;
                }

                var workspaceGlobalDemoInfo = sInfo.globalDemoInfo;
                if (workspaceGlobalDemoInfo == null)
                {
                    return;
                }

                if (orchestrator != null && orchestrator != sInfo.globalDemoInfo)
                {
                    DemoKitLog.Warning($"The {nameof(GlobalDemoInfo)} attached to {nameof(DemoOrchestrator)} does not match the one specified during setup.", orchestrator);
                    return;
                }
            });

        private static void Initialize()
        {
            EditorApplication.delayCall -= Initialize;

            RunWithCurrentSceneIsDemoMainScene((scene, aInfo) =>
            {
                CheckSceneSettings(scene);
            });
        }

        static DemoKitSceneUtils()
        {
            EditorApplication.delayCall -= Initialize;
            EditorApplication.delayCall += Initialize;
        }


    }
}