// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEngine;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A high-level section layout that consolidates all steps required for demo distribution.
    /// It orchestrates the folder selection, option configuration, and file generation processes 
    /// by nesting multiple <see cref="BaseStepLayout"/> implementations. This section serves as 
    /// the final workflow stage, guiding the developer from initial path setup to the 
    /// ultimate distribution of generated files.
    /// </summary>
    internal class DistributionSetupLayout : BaseSectionLayout
    {
        internal override string Title => "Distribution Setup";

        internal override string Description => "Follow the steps below to set up the demo for distribution.";

        internal override Color? BgColor => new (0.3f, 0.7f, 0.5f, 0.25f);


        private readonly DistributionFolderSelectLayout _distributionFolderSelectLayout = new();
        private readonly PrepareDistributionFilesLayout _prepareDistributionFilesLayout = new();
        private readonly DistributionOptionSettingLayout _distributionOptionSettingLayout = new();
        private readonly CompletePrepareDistributionFilesLayout _completePrepareDistributionFilesLayout = new();

        internal override bool ContentLayout()
        {
            var needsRepaint = false;

            needsRepaint |= _distributionFolderSelectLayout.ShowStep(1);

            DemoKitGUI.LargeSpace();

            needsRepaint |= _distributionOptionSettingLayout.ShowStep(2);

            DemoKitGUI.LargeSpace();

            
            needsRepaint |= _prepareDistributionFilesLayout.ShowStep(3);

            DemoKitGUI.LargeSpace();

            needsRepaint |= _completePrepareDistributionFilesLayout.ShowStep(4);

            return needsRepaint;
        }
    }
}
