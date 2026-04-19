// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEngine;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A section layout that handles the initial environment configuration for a new demo.
    /// It guides the developer through naming the demo, selecting a valid workspace folder, 
    /// and generating the necessary baseline assets. It includes safety checks to ensure 
    /// the workspace is correctly located within the Unity Assets directory before 
    /// allowing the asset generation step to proceed.
    /// </summary>
    internal class InitialSetupLayout : BaseSectionLayout
    {
        internal override string Title => "Initial Setup";

        internal override string Description => "Follow the steps below to initialize the demo environment for your project.";

        internal override Color? BgColor => new(0.3f, 0.5f, 0.8f, 0.35f);

        private readonly DemoNameInputLayout _demoNameInputLayout = new();
        private readonly DemoWorkspaceSelectLayout _demoWorkspaceSelectLayout = new();
        private readonly SetupRequiredAssetsLayout _initialAssetsSetupLayout = new();

        internal override bool ContentLayout()
        {
            var needsRepaint = false;

            needsRepaint |= _demoNameInputLayout.ShowStep(1);

            DemoKitGUI.LargeSpace();

            needsRepaint |= _demoWorkspaceSelectLayout.ShowStep(2);
            
            DemoKitGUI.LargeSpace();

            DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                var isValidPath = sInfo.IsValidWorkspacePath;
                
                if (!isValidPath)
                {
                    DemoKitGUI.ErrorBox("Can't proceed follow steps because the workspace folder is not under Assets folder.");
                }

                DemoKitGUI.EnabledSwitcher(
                    isValidPath,
                    enabled =>
                    {
                        needsRepaint |= _initialAssetsSetupLayout.ShowStep(3);
                    }
                );
            });

            /*

            if (!_demoWorkspaceSelectLayout.IsValidFolderPath)
            {
                DemoKitGUI.ErrorBox("Can't proceed follow steps because the workspace folder is not under Assets folder.");
            }

            DemoKitGUI.EnabledSwitcher(

            );

            DemoKitGUI.WithAlpha(
                _demoWorkspaceSelectLayout.IsValidFolderPath ? 1f : 0.5f,
                () => needsRepaint |= _initialAssetsSetupLayout.ShowStep(3)
            );
            */

            return needsRepaint;
        }
    }
}
