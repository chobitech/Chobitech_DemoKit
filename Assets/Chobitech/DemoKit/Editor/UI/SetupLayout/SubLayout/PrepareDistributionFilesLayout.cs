// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEngine.Events;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A concrete implementation of <see cref="BaseStepLayout"/> that handles the final generation 
    /// of distribution assets. It orchestrates the creation of a Unity package for core logic, 
    /// a zip archive for workspace assets, and a C# initializer script. It also includes 
    /// an expandable information section to explain the purpose of each generated file to the user.
    /// </summary>
    internal class PrepareDistributionFilesLayout : BaseStepLayout
    {

        private const string noticeAfterPreparingExpandStateKey = "noticeAfterPreparingExpandState";

        internal override string Title => "Generate Distribution Files";

        internal override string Description => "Generate files for distribution by clicking the button below.";

        protected override UnityAction SubDescriptionLayout => () =>
        {
            ExplainLayout();
        };

        private readonly DemoKitGUI.Expandable _subDescrExpandable = new("[Notice] Generated files");


        private void ExplainLayout()
        {
            DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                var demoName = sInfo.DisplayDemoName;

                _subDescrExpandable.ShowLayout(
                    () =>
                    {
                        DemoKitGUI.Label($"Upon completion, the following files will be generated in the distribution folder:");
                        DemoKitGUI.IndentBlock(
                            () =>
                            {
                                DemoKitGUI.InBulletPoints(
                                    $"<b>{DemoKitUtils.DefaultValues.InitializerCodeFileNameSuffix}</b>: A C# script to initialize your demo at the destination.",
                                    $"<b>{DemoKitUtils.DefaultValues.DistributionAssetsZipFileName}</b>: A zip archive of workspace assets in ({sInfo.RelativeWorkspaceFolderPath}). Hidden and system files are excluded.",
                                    $"<b>{DemoKitUtils.DefaultValues.DistributionInternalCodePackageName}</b>: A Unity package containing the core <b>Chobitech.DemoKit</b> components."
                                );
                            }
                        );
                    },
                    DemoKitUserSettings.GetExpandableState(noticeAfterPreparingExpandStateKey),
                    expanded => DemoKitUserSettings.SetExpandableState(noticeAfterPreparingExpandStateKey, expanded)
                );
            });

        }


        private void CopyAssetsToDistributionFolder(DemoSetupInfo sInfo)
        {
            
            var internalPackageGenerated = DemoDistributionUtils.ExportInternalCodePackage(sInfo.DistributionInternalUnityPackageFullPath);
            if (!internalPackageGenerated)
            {
                DemoKitLog.Error($"Failed to generate {DemoKitUtils.DefaultValues.DistributionInternalCodePackageName}.");
                return;
            }

            var tempPath = DemoKitPaths.GenerateTempFolderFullPath();
            var zipGenerated = DemoDistributionUtils.GenerateZipArchiveOfDistributionFolder(sInfo, tempPath);
            if (!zipGenerated)
            {
                DemoKitLog.Error($"Failed to generate {sInfo.DistributionAssetsZipFileName}.");
                return;
            }

            var initializeCodeGenerated = DemoDistributionUtils.GenerateAutoInitializerCode(sInfo);

            if (!initializeCodeGenerated)
            {
                DemoKitLog.Error($"Failed to generate {sInfo.DistributionDemoInitializerCodeFileName}.");
                return;
            }

            DemoKitLog.Info($"Preparing {sInfo.demoName} is completed. Please check the distribution folder: {sInfo.distributionFolderPath}");
        }

        protected override bool InnerStepLayout()
        {
            return DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                var distrPath = sInfo.distributionFolderPath;

                DemoKitGUI.WarningBox("Existing files will be overwritten.");

                DemoKitGUI.Button(
                    "Generate Distribution Files",
                    () => CopyAssetsToDistributionFolder(sInfo),
                    !string.IsNullOrEmpty(distrPath)
                );

                return false;
            });
        }
    }
}
