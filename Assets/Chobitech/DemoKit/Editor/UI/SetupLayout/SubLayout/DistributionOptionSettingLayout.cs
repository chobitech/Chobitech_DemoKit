// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEditor;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A concrete implementation of <see cref="BaseStepLayout"/> for configuring distribution-specific options.
    /// It primarily manages the removal of redundant Unity "EventSystem" objects to prevent 
    /// UI input conflicts when the demo is integrated into other projects. It also 
    /// provides contextual warnings about common distribution pitfalls.
    /// </summary>
    internal class DistributionOptionSettingLayout : BaseStepLayout
    {
        internal override string Title => "Distribution Option Setting";

        internal override string Description => "";

        private bool _isRemoveEventSystem = true;

        protected override bool InnerStepLayout()
        {
            var needsRepaint = false;

            needsRepaint |= DemoKitGUI.WithChangeCheck(
                () =>
                {
                    _isRemoveEventSystem = EditorGUILayout.ToggleLeft("Remove EventSystem object from the scene.", _isRemoveEventSystem);
                },
                () =>
                {
                    
                }
            );

            DemoKitGUI.InfoBox($"Distributing the demo with an EventSystem included may cause compatibility issues or errors depending on the user's environment.");

            return needsRepaint;
        }
    }
}