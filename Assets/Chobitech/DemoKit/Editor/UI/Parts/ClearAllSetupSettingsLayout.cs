// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEditor;
using UnityEngine;

namespace Chobitech.DemoKit.Editor
{
    internal class ClearAllSetupSettingsLayout
    {
        internal bool ShowLayout()
        {
            var needsRepaint = false;

            DemoKitGUI.AlignRight(
                () =>
                {
                    GUILayout.FlexibleSpace();

                    DemoKitGUI.Label("To clear setup settings: ");

                    DemoKitGUI.Button(
                        "Clear Settings",
                        () =>
                        {
                            var execClear = EditorUtility.DisplayDialog(
                                "Clear Settings Confirmation",
                                "Are you sure you want to clear all setup settings? (This action cannot be undone.)",
                                "Yes",
                                "No"
                            );

                            if (execClear)
                            {
                                DemoSetupSettings.ClearCurrentSetupInfo();
                                DemoSetupSettings.Save();
                            }

                            needsRepaint |= execClear;
                        },
                        true
                    );
                }
            );

            

            return needsRepaint;
        }
    }
}
