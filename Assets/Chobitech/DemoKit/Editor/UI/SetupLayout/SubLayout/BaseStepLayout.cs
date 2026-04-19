// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System;
using UnityEngine.Events;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] Provides a base abstraction for multi-step workflow layouts in the Editor UI.
    /// It manages the standardized rendering of step headers with optional progress 
    /// check-marks, main descriptions, and flexible sub-description areas. This ensures 
    /// that complex setup processes remain consistent and visually guided throughout 
    /// the DemoKit's configuration windows.
    /// </summary>
    internal abstract class BaseStepLayout
    {
        internal static bool ShowStepLayout(
            int stepNumber,
            string title,
            bool? stepChecked,
            Func<bool> contentLayout,
            string description = null,
            UnityAction subDescriptionLayout = null
        )
        {
            DemoKitGUI.WithCheckMark(stepChecked, () => DemoKitGUI.SmallHeading($"[Step {stepNumber}] - {title}"));

            if (!string.IsNullOrEmpty(description))
            {
                DemoKitGUI.Label(description);
            }

            if (subDescriptionLayout != null)
            {
                DemoKitGUI.SmallSpace();
                subDescriptionLayout();
            }

            DemoKitGUI.SmallSpace();

            return contentLayout?.Invoke() ?? false;
        }

        protected virtual bool? GetStepChecked() => null;

        internal abstract string Title { get; }
        internal abstract string Description { get; }

        protected virtual UnityAction SubDescriptionLayout { get; }

        protected abstract bool InnerStepLayout();

        internal bool ShowStep(int stepNumber)
            => ShowStepLayout(stepNumber, Title, GetStepChecked(), InnerStepLayout, Description, SubDescriptionLayout);
    }
}