// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// [Internal] Represents the header UI component of the DemoKit.
    /// It manages the branding elements of the kit, including the display of the main icon 
    /// and the global demonstration title. Inherits from <see cref="RectTransformCachedMonoBehaviour"/> 
    /// to provide efficient size access for layout calculations.
    /// </summary>
    internal class DemoHeader : RectTransformCachedMonoBehaviour
    {
        [SerializeField]
        private Image demoIcon;

        [SerializeField]
        private TMP_Text demoTitle;

        internal void SetIcon(Sprite icon)
        {
            this.RunIfUnityObjectIsNotNull(demoIcon, i =>
            {
                var showIcon = icon != null;
                i.enabled = showIcon;
                if (showIcon)
                {
                    i.sprite = icon;
                }
            });
        }

        internal void SetTitle(string title)
        {
            this.RunIfUnityObjectIsNotNull(demoTitle, t => t.text = title);
        }
    }
}
