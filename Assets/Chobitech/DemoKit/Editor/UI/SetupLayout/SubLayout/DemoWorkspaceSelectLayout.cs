// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A concrete implementation of <see cref="BaseStepLayout"/> for selecting the demo's workspace directory.
    /// It utilizes <see cref="FolderSelectLayout"/> to provide a folder picker UI and ensures 
    /// the chosen path is stored within <see cref="DemoSetupSettings"/>. This step tracks 
    /// completion based on whether a valid folder path has been assigned.
    /// </summary>
    internal class DemoWorkspaceSelectLayout : BaseStepLayout
    {
        internal override string Title => "Select your workspace folder";

        internal override string Description => "";

        private readonly FolderSelectLayout _folderSelectLayout = new();

        private string _workspaceFolderPath;

        //internal bool IsValidFolderPath => DemoKitEditorUtils.IsUnderAssetsFolder(_workspaceFolderPath);

        protected override bool? GetStepChecked()
        {
            return DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                return sInfo.IsValidWorkspacePath && !string.IsNullOrEmpty(sInfo.workspaceFolderPath);
            });
        }

        protected override bool InnerStepLayout()
        {
            return DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                _workspaceFolderPath = sInfo.workspaceFolderPath;

                return _folderSelectLayout.ShowLayout(
                    "Select Workspace Folder",
                    sInfo.RelativeWorkspaceFolderPath,
                    path =>
                    {
                        sInfo.workspaceFolderPath = path;
                        DemoSetupSettings.Save();
                    },
                    true
                );
            });

        }
    }
}