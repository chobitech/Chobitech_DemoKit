// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEditor;
using UnityEngine.Events;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A dedicated UI layout component for selecting and validating project folders.
    /// It provides a combined interface of a text field for manual path entry and a 
    /// "Select Folder" button that opens a native OS dialog. It ensures paths are 
    /// formatted correctly (using forward slashes) and validates that selected folders 
    /// reside within the Unity "Assets" directory when required.
    /// </summary>
    internal class FolderSelectLayout
    {

        private string _innerFolderPath;

        internal string FolderFullPath => _innerFolderPath;

        internal string FolderRelativePath => !string.IsNullOrEmpty(_innerFolderPath)
            ? FileUtil.GetProjectRelativePath(_innerFolderPath)
            : null;

        //internal bool IsValidFolderPath => DemoKitEditorUtils.IsUnderAssetsFolder(_innerFolderPath);
        internal bool IsValidFolderPath => DemoKitPaths.IsUnderAssetsFolder(_innerFolderPath);


        internal bool ShowLayout(string dialogTitle, string folderPath, UnityAction<string> onChanged, bool mustInAssetsFolder = true)
        {
            var isValueChanged = false;

            _innerFolderPath = folderPath;

            isValueChanged |= DemoKitGUI.TextField(
                input =>
                {
                    _innerFolderPath = DemoKitPaths.ConvertToSlash(input);
                    onChanged?.Invoke(_innerFolderPath);
                },
                _innerFolderPath
            );

            if (mustInAssetsFolder && !IsValidFolderPath)
            {
                DemoKitGUI.ErrorBox($"Select a folder under the Assets folder.");
            }

            DemoKitGUI.Button(
                "Select Folder",
                () =>
                {
                    var path = DemoKitEditorUtils.SelectFolderInAssets(dialogTitle, _innerFolderPath);
                    
                    var pathChanged = !string.IsNullOrEmpty(path) && path != _innerFolderPath;

                    isValueChanged |= pathChanged;

                    if (pathChanged)
                    {
                        _innerFolderPath = path;
                        onChanged?.Invoke(_innerFolderPath);
                    }

                    if (mustInAssetsFolder && pathChanged && !IsValidFolderPath)
                    {
                        DemoKitLog.Error($"{_innerFolderPath} is not under the Assets folder.");
                    }
                },
                true
            );
            
            return isValueChanged;
        }
    }
}
