// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEngine;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A base abstract class for standardized section layouts in the DemoKit Editor UI.
    /// It enforces a consistent visual hierarchy by automating the rendering of 
    /// section numbers, titles, descriptions, and indentation for content. 
    /// This structure ensures that various setup or configuration panels share 
    /// a professional and unified appearance.
    /// </summary>
    internal abstract class BaseSectionLayout
    {
        private static bool InnerShowSectionLayout(int sectionNumber, string title, Color? titleBgColor, string description, System.Func<bool> contentLayout, bool showBottomSeparator)
        {
            DemoKitGUI.ColoredArea(
                titleBgColor,
                () =>
                {
                    DemoKitGUI.MediumHeading($"{sectionNumber}. {title}");
                    return false;
                }
            );


            if (description != null)
            {
                DemoKitGUI.SmallSpace();
                DemoKitGUI.Label(description);
            }

            DemoKitGUI.SmallSpace();

            var needRepaint = false;

            DemoKitGUI.IndentBlock(() => needRepaint = contentLayout?.Invoke() ?? false);

            if (showBottomSeparator)
            {
                DemoKitGUI.SeparatorAndSpace();
            }

            return needRepaint;
        }

        internal virtual Color? BgColor { get; }
        
        internal abstract string Title { get; }
        internal abstract string Description { get; }

        internal abstract bool ContentLayout();

        internal bool ShowSectionLayout(int sectionNumber, bool showBottomSeparator = false)
            => InnerShowSectionLayout(sectionNumber, Title, BgColor, Description, ContentLayout, showBottomSeparator);

    }
}