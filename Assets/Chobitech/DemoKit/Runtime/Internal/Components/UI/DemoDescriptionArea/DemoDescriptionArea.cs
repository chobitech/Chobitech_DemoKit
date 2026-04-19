// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEngine;
using UnityEngine.UI;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// [Internal] Manages the description UI panel, which displays both global project info and specific demo details.
    /// It coordinates with a <see cref="BaseDrawer"/> to handle panel expansion/retraction and updates 
    /// visual indicators like button icons based on the current drawer state.
    /// </summary>
    [RequireComponent(typeof(BaseDrawer))]
    internal class DemoDescriptionArea : MonoBehaviour
    {
        [Header("Components and Assets")]
        [SerializeField]
        private ScrollTextArea globalDemoDescriptionArea;

        [SerializeField]
        private ScrollTextArea currentDemoDescriptionArea;

        [SerializeField]
        private RectTransform drawerButtonArea;

        [SerializeField]
        private Image drawerButtonIcon;

        [SerializeField]
        private Sprite drawerOpenedIcon;

        [SerializeField]
        private Sprite drawerClosedIcon;

        internal BaseDrawer BaseDrawer { get; private set; }

        void Awake()
        {
            BaseDrawer = GetComponent<BaseDrawer>();

            this.RunIfUnityObjectIsNotNull(BaseDrawer, d =>
            {
                d.onDrawerStateChanged.AddListener(OnDrawerStageChanged);
            });
        }

        protected void OnDrawerStageChanged(bool isOpened)
        {
            this.RunIfUnityObjectIsNotNull(drawerButtonIcon, icon =>
            {
                icon.sprite = isOpened ? drawerOpenedIcon : drawerClosedIcon;
            });
        }

        internal void OpenOrCloseDrawer()
        {
            this.RunIfUnityObjectIsNotNull(BaseDrawer, d => d.ToggleOpenOrClose());
        }

        internal void SetGlobalDescription(string text)
        {
            this.RunIfUnityObjectIsNotNull(globalDemoDescriptionArea, a => a.SetContentText(text));
        }

        internal void SetCurrentDemoDescription(string text)
        {
            this.RunIfUnityObjectIsNotNull(currentDemoDescriptionArea, c => c.SetContentText(text));
        }

        internal void SetCurrentDemoTitle(string title)
        {
            this.RunIfUnityObjectIsNotNull(currentDemoDescriptionArea, c => c.SetTitle(title));
        }
    }
}
