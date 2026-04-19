// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A static UI component that renders the top-level header for the DemoKit Setup window.
    /// It displays the tool title, a brief introductory overview of the distribution workflow, 
    /// and a helpful navigation hint showing users how to re-open the window via the Unity menu.
    /// </summary>
    internal static class HeaderLayout
    {


        internal static void ShowLayout()
        {
            DemoKitGUI.ColoredArea(
                new(0.4f, 0.2f, 0.7f, 0.75f),
                () =>
                {
                    DemoKitGUI.MediumHeading("<color=#ffffff>Chobitech.DemoKit Demo Setup</color>");
                    return false;
                }
            );
            //DemoKitGUI.MediumHeading("Chobitech.DemoKit Demo Setup");

            DemoKitGUI.SmallSpace();

            DemoKitGUI.Label($"In this window, you can set up your demo to distribute included in your package or project with <b>Chobitech.DemoKit</b> by below steps.");

            DemoKitGUI.SmallSpace();

            DemoKitGUI.InfoBox($"To show this window, select the menu {DemoKitEditorMenu.GetMenuPathNavigationString(DemoKitEditorMenu.SetupWindowPath)}.");

            DemoKitGUI.SeparatorAndSpace();
        }
    }
}