// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// [Internal] Manages the log display area within the DemoKit, acting as a controller for log visibility and scrolling.
    /// It synchronizes with a <see cref="BaseDrawer"/> for panel transitions and handles intelligent auto-scroll behavior,
    /// ensuring that new logs are displayed automatically unless the user is manually inspecting the log history.
    /// </summary>
    [RequireComponent(typeof(BaseDrawer))]
    internal class DemoLogArea : MonoBehaviour
    {
        internal BaseDrawer BaseDrawer { get; private set; }

        [Header("Components")]
        [SerializeField]
        private ScrollTextArea logArea;

        [SerializeField]
        private RectTransform buttonIcon;

        [SerializeField]
        private TMP_Text buttonLabel;

        [SerializeField]
        private Button scrollToTopButton;

        [SerializeField]
        private Button autoScrollButton;
        [SerializeField]
        private Image autoScrollIcon;
        [SerializeField]
        private Image disableAutoScrollIcon;

        private bool _isUserScrolling = false;

        void Awake()
        {
            BaseDrawer = GetComponent<BaseDrawer>();

            this.RunIfUnityObjectIsNotNull(BaseDrawer, d =>
            {
                d.onDrawerStateChanged.AddListener(OnDrawerStateChanged);
                d.onDrawerMoving.AddListener(OnDrawerMove);
            });
            
            this.RunIfUnityObjectIsNotNull(logArea, la =>
            {
                la.onUserScrollingBegin.AddListener(DisableAutoScroll);
            });

            this.RunIfUnityObjectIsNotNull(autoScrollButton, b => b.onClick.AddListener(OnAutoScrollButtonClicked));
            this.RunIfUnityObjectIsNotNull(scrollToTopButton, b => b.onClick.AddListener(OnScrollToTopButtonClicked));
        }

        private void ChangeAutoScrollState(bool enabled)
        {
            this.RunIfUnityObjectIsNotNull(autoScrollIcon, asIcon =>
            {
                var c = asIcon.color;
                c.a = enabled ? 1f : 0.3f;
                asIcon.color = c;
            });
            this.RunIfUnityObjectIsNotNull(disableAutoScrollIcon, dIcon => dIcon.gameObject.SetActive(!enabled));
        }

        internal void DisableAutoScroll()
        {
            _isUserScrolling = true;
            ChangeAutoScrollState(false);
        }

        internal void EnableAutoScroll()
        {
            _isUserScrolling = false;
            ChangeAutoScrollState(true);
        }

        private void OnAutoScrollButtonClicked()
        {
            if (_isUserScrolling)
            {
                EnableAutoScroll();
            }
            else
            {
                DisableAutoScroll();
            }
        }

        private void OnScrollToTopButtonClicked()
        {
            DisableAutoScroll();
            this.RunIfUnityObjectIsNotNull(logArea, la => la.ScrollToTop());
        }

        internal void SetLogText(string log)
        {
            this.RunIfUnityObjectIsNotNull(logArea, la =>
            {
                la.SetContentText(log);
                if (!_isUserScrolling)
                {
                    la.ScrollToBottom();
                }
            });
        }

        internal void OnOpenCloseButtonClicked()
        {
            this.RunIfUnityObjectIsNotNull(BaseDrawer, d => d.ToggleOpenOrClose());
        }

        internal void OnDrawerStateChanged(bool opened)
        {
            this.RunIfUnityObjectIsNotNull(buttonLabel, label =>
            {
                label.text = $"{(opened ? "Close" : "Open")} Log";
            });
        }

        internal void OnDrawerMove(float rate)
        {
            this.RunIfUnityObjectIsNotNull(buttonIcon, icon =>
            {
                icon.localRotation = Quaternion.AngleAxis(rate * 180f, Vector3.forward);
            });
        }
    }
}
