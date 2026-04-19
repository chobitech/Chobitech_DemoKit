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
    /// [Internal] Manages persistent user preferences and editor states for the DemoKit.
    /// Utilizing <see cref="ScriptableSingleton{T}"/> with a defined file path in ProjectSettings, 
    /// it stores information such as the first-time startup flag, scene-specific EventSystem 
    /// configuration prompts, and the expandable state of UI sections to ensure a consistent 
    /// developer experience across sessions.
    /// </summary>
    [FilePath("ProjectSettings/DemoKitUserSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal class DemoKitUserSettings : ScriptableSingleton<DemoKitUserSettings>, ISerializationCallbackReceiver
    {
        [SerializeField]
        internal bool firstStartupFinished = false;

        [SerializeField]
        private List<string> doNotAskToAddEventSystemSceneGuids = new();
        private HashSet<string> _doNotAskToAddEventSystemSceneGuidSet = new();

        internal static bool IsDoNotAskAddEventSystem(string guid)
            => instance._doNotAskToAddEventSystemSceneGuidSet.Contains(guid);
        
        internal static bool IsDoNotAskAddEventSystem(Scene scene)
        {
            var guid = DemoKitEditorUtils.GetSceneGUID(scene);
            if (string.IsNullOrEmpty(guid))
            {
                return true;
            }

            return instance._doNotAskToAddEventSystemSceneGuidSet.Contains(guid);
        }
        
        internal static void SetIsDoNotAskAddEventSystem(string guid, bool b)
        {
            if (b)
            {
                instance._doNotAskToAddEventSystemSceneGuidSet.Add(guid);
            }
            else
            {
                instance._doNotAskToAddEventSystemSceneGuidSet.Remove(guid);
            }
        }
        
        [SerializeField]
        private SerializableMap<string, bool> expandableStateMap = new();

        internal static bool GetExpandableState(string key)
        {
            if (instance.expandableStateMap.TryGetValue(key, out var isExpanded))
            {
                return isExpanded;
            }
            return false;
        }


        internal static void SetExpandableState(string key, bool isExpanded)
        {
            instance.expandableStateMap[key] = isExpanded;
        }

        internal static void Save() => instance.Save(true);


        public void OnBeforeSerialize()
        {
            doNotAskToAddEventSystemSceneGuids = _doNotAskToAddEventSystemSceneGuidSet.ToList();
        }

        public void OnAfterDeserialize()
        {
            _doNotAskToAddEventSystemSceneGuidSet = new(doNotAskToAddEventSystemSceneGuids);
        }



        internal static void Update(System.Func<DemoKitUserSettings, bool> updater)
        {
            if(updater?.Invoke(instance) == true)
            {
                Save();
            }
        }
    }
}