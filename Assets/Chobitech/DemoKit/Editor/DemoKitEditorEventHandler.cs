// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using UnityEditor;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] Orchestrates global Editor lifecycle events for the DemoKit.
    /// It automatically triggers the setup window on the first project startup 
    /// and ensures that all user preferences and demo configuration settings 
    /// are persisted to disk when the Unity Editor is closed.
    /// </summary>
    [InitializeOnLoad]
    internal static class DemoKitEditorEventHandler
    {
        private static void DelayCall()
        {
            EditorApplication.delayCall -= DelayCall;

            DemoKitUserSettings.Update(s =>
            {
                if (!s.firstStartupFinished)
                {
                    DemoKitSetupWindow.ShowSetupWindow();
                    s.firstStartupFinished = true;
                    return true;
                }

                return false;
            });
            
            if (!EditorWindow.HasOpenInstances<DemoKitSetupWindow>())
            {
                DemoKitLog.Info($"To open the demo setup window, select the menu: {DemoKitEditorMenu.GetMenuPathNavigationString(DemoKitEditorMenu.SetupWindowPath)}, or open [Project Settings] > {DemoKitEditorMenu.GetMenuPathNavigationString(DemoKitEditorMenu.DemoKitProjectSettingsPath)}");
            }
        }

        private static void OnQuit()
        {
            EditorApplication.quitting -= OnQuit;
            
            DemoKitUserSettings.Save();
            DemoSetupSettings.Save();
        }

        static DemoKitEditorEventHandler()
        {
            EditorApplication.delayCall -= DelayCall;
            EditorApplication.delayCall += DelayCall;
            
            EditorApplication.quitting -= OnQuit;
            EditorApplication.quitting += OnQuit;
        }
    }
}