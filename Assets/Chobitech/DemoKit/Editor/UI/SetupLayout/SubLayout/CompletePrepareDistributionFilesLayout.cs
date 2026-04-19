// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.IO;
using UnityEditor;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] Manages the final layout for the distribution step, providing the UI to open the 
    /// distribution folder and displaying critical usage instructions for the generated files.
    /// </summary>
    internal class CompletePrepareDistributionFilesLayout : BaseStepLayout
    {
        internal override string Title => "Distribute Your Demo";

        internal override string Description => null;

        protected override bool InnerStepLayout()
        {
            var needsRepaint = false;

            DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                DemoKitGUI.Label($"Distribute the files in the distribution folder.");

                DemoKitGUI.SmallSpace();

                DemoKitGUI.Button(
                    "Open Distribution Folder",
                    () =>
                    {
                        if (!Directory.Exists(sInfo.distributionFolderPath))
                        {
                            DemoKitLog.Error($"The distribution folder '{sInfo.distributionFolderPath}' not exists.");
                            return;
                        }

                        EditorUtility.RevealInFinder(sInfo.distributionFolderPath);
                    },
                    !string.IsNullOrEmpty(sInfo.distributionFolderPath)
                );

                DemoKitGUI.SmallSpace();

                DemoKitGUI.Label($"At the destination, the AutoInitializer script will launch a setup window, allowing users to easily initialize your demo assets.");

                DemoKitGUI.ErrorBox(
                            $@"Strict adherence to the following is required for the demo to function correctly:
{DemoKitGUI.Bullet} Do not delete or separate the generated files from the folder.
{DemoKitGUI.Bullet} Do not rename any of the generated files.
{DemoKitGUI.Bullet} Do not modify or edit the content of the generated files.");
                    });

            return needsRepaint;
        }
    }
}
