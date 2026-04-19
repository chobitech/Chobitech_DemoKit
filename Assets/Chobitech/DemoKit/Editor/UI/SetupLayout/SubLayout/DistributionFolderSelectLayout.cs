// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A concrete implementation of <see cref="BaseStepLayout"/> for defining the distribution output directory.
    /// Unlike the workspace selection, this step allows folders outside the Unity "Assets" 
    /// directory (e.g., a local build or export folder) to facilitate flexible distribution 
    /// workflows. It ensures the chosen path is persisted in <see cref="DemoSetupSettings"/>.
    /// </summary>
    internal class DistributionFolderSelectLayout : BaseStepLayout
    {
        internal override string Title => "Select The Demo Distribution Folder";

        internal override string Description => "";

        private readonly FolderSelectLayout _folderSelectLayout = new();

        protected override bool? GetStepChecked()
        {
            return DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo => !string.IsNullOrEmpty(sInfo.distributionFolderPath));
        }

        protected override bool InnerStepLayout()
        {
            return DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                var needsRepaint = false;

                needsRepaint |= _folderSelectLayout.ShowLayout(
                    "Select Distribution Folder",
                    sInfo.distributionFolderPath,
                    path =>
                    {
                        sInfo.distributionFolderPath = path;
                        DemoSetupSettings.Save();
                    },
                    false
                );

                return needsRepaint;
            });
            
        }
    }
}