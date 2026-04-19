// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A UI component that displays a step-by-step implementation guide for developers.
    /// It provides clear instructions on how to create individual demos using the 
    /// provided base classes (Async/Coroutine), how to configure demo metadata, 
    /// and the correct way to structure the scene hierarchy for automatic detection.
    /// </summary>
    internal class DemoImplementationGuideLayout
    {
        private void ConfigureGlobalDemoInfoLayout(int number, DemoSetupInfo sInfo)
        {
            DemoKitGUI.Label($"<b>{number}) Configure GlobalDemoInfo</b>");

            DemoKitGUI.IndentBlock(() =>
            {
                DemoKitGUI.Label($"Set the title, icon and description of your demo in <b>GlobalDemoInfo</b> asset.");
                DemoKitGUI.Label($"(Optional) You can set your custom color tags with <b>CustomColorTag</b> in <b>GlobalDemoInfo</b> asset.");
            });
        }

        private void ImplementIndividualDemoInfoLayout(int number, DemoSetupInfo sInfo)
        {
            DemoKitGUI.Label($"<b>{number}) Implement Individual Demos</b>");

            DemoKitGUI.IndentBlock(() =>
            {
                DemoKitGUI.Label($"Create individual demos inheriting <b>{nameof(AsyncDemoBase)}</b> or <b>{nameof(CoroutineDemoBase)}</b>, and attach <b>{nameof(IndividualDemoInfo)}</b> to each demo objects.");
                DemoKitGUI.InBulletPoints(
                    $"<b>{nameof(AsyncDemoBase)}</b>: Runs demo as async task.",
                    $"<b>{nameof(CoroutineDemoBase)}</b>: Runs demo as Coroutine.",
                    $"<b>{nameof(IndividualDemoInfo)}</b>: The asset information of the individual demo. To generate this asset, select the menu: {DemoKitEditorMenu.GetAssetCreateMenuPathString("Individual Demo Information")}."
                );
            });
        }

        private void PlaceIndividualDemosLayout(int number, DemoSetupInfo sInfo)
        {
            DemoKitGUI.Label($"<b>{number}) Place Individual Demos</b>");
            DemoKitGUI.IndentBlock(
                () =>
                {
                    DemoKitGUI.Label($"Place Individual Demos under the <b>Demo Container</b> object in the demo main scene. Individual Demos are detected automatically.");
                }
            );

        }


        internal void ShowLayout()
        {
            DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                ConfigureGlobalDemoInfoLayout(1, sInfo);

                ImplementIndividualDemoInfoLayout(2, sInfo);

                PlaceIndividualDemosLayout(3, sInfo);
            });

            DemoKitGUI.SmallSpace();

            DemoKitGUI.Label($"<b>* For More Details</b>");
            DemoKitGUI.Label($"Please check the documents of <b>{DemoKitUtils.PackageName}</b>:");
            DemoKitGUI.IndentBlock(() =>
            {
                DemoKitGUI.LinkLabel(DemoKitUtils.DemoKitDocumentUrl);
            });
        }
    }
}