// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] Acts as the primary orchestrator for the setup UI, aggregating and displaying 
    /// the three core stages: Initial Setup, Demo Implementation, and Distribution Setup.
    /// </summary>
    internal class SetupMainLayout
    {
        private readonly CheckRequiredAssetsLayout _checkRequiredAssetsLayout = new();
        private readonly InitialSetupLayout _demoInitialSetupLayout = new();
        private readonly ImplementYourDemoLayout _openDemoSceneLayout = new();
        private readonly DistributionSetupLayout _distributionSetupLayout = new();

        private readonly ClearAllSetupSettingsLayout _clearAllSetupSettingsLayout = new();

        internal bool ShowLayout()
        {
            var needsRepaint = false;

            needsRepaint |= _clearAllSetupSettingsLayout.ShowLayout();

            DemoKitGUI.Separator();

            needsRepaint |= _checkRequiredAssetsLayout.ShowSectionLayout(0, true);

            DemoKitGUI.WhenTMPImported(
                b =>
                {
                    needsRepaint |= _demoInitialSetupLayout.ShowSectionLayout(1, true);
                    DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
                    {
                        DemoKitGUI.EnabledSwitcher(
                            sInfo.IsValidWorkspacePath && sInfo.IsRequiredAssetsSet,
                            enabled =>
                            {
                                needsRepaint |= _openDemoSceneLayout.ShowSectionLayout(2, true);
                                needsRepaint |= _distributionSetupLayout.ShowSectionLayout(3);
                            }
                        );
                    });
                }
            );

            return needsRepaint;
        }
    }
}
