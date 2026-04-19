// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using UnityEditor.SceneManagement;
using UnityEngine;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A section layout that provides a direct entry point for the demo implementation phase.
    /// It features a button to open the designated demo main scene with built-in scene 
    /// saving prompts and state checks. Additionally, it embeds an expandable 
    /// implementation guide to assist developers with technical details while they work.
    /// </summary>
    internal class ImplementYourDemoLayout : BaseSectionLayout
    {
        private const string explainExpandStateKey = "implementYourDemoExplainExpandState";

        internal override string Title => "Implement Your Demo";

        internal override string Description => "Open the demo scene to start implementing your own features.";

        internal override Color? BgColor => new (0.8f, 0.6f, 0.2f, 0.2f);


        private readonly DemoKitGUI.Expandable _explainExpandable = new("Quick Implementation Guide");

        private readonly DemoImplementationGuideLayout _demoImplementationGuideLayout = new();

        internal override bool ContentLayout()
        {
            DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                DemoKitGUI.Button(
                    "Open Demo Scene",
                    () =>
                    {
                        if (sInfo.demoMainScene == null)
                        {
                            DemoKitLog.Error($"Demo main scene is not found.");
                            return;
                        }

                        var demoScenePath = DemoKitEditorUtils.GetAssetPath(sInfo.demoMainScene);

                        var currentScenePath = EditorSceneManager.GetActiveScene().path;
                        if (currentScenePath == demoScenePath)
                        {
                            return;
                        }

                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(demoScenePath);
                            DemoKitLog.Info($"Open your demo scene: {System.IO.Path.GetFileNameWithoutExtension(sInfo.DemoMainSceneFileName)}");
                        }
                    },
                    sInfo.IsRequiredAssetsSet && sInfo.demoMainScene != null
                );
            });

            

            DemoKitGUI.SmallSpace();

            _explainExpandable.ShowLayout(
                () =>
                {
                    _demoImplementationGuideLayout.ShowLayout();
                },
                DemoKitUserSettings.GetExpandableState(explainExpandStateKey),
                expanded =>
                {
                    DemoKitUserSettings.SetExpandableState(explainExpandStateKey, expanded);
                }
            );

            return false;
        }
    }
}
