// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// [Internal] A specialized UI component providing a scrollable text area designed for logs and descriptions.
    /// It coordinates automatic scrolling, text wrapping, and manual user interaction detection.
    /// Uses <see cref="UserScrollDetector"/> and <see cref="UserScrollBarTouchDetector"/> to intelligently 
    /// pause auto-scroll behavior when the user is actively reading or dragging.
    /// </summary>
    internal class ScrollTextArea : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private bool wrapHorizontalText = true;

        [Header("Components")]
        [SerializeField]
        private TMP_Text titleTmpText;

        [SerializeField]
        private TMP_Text textAreaTmpText;

        [SerializeField]
        private ScrollRect scrollRect;

        internal ScrollRect SelfScrollRect => scrollRect;

        [SerializeField]
        private ContentSizeFitter contentSizeFitter;

        [SerializeField]
        private RectTransform contentRectTransform;

        [SerializeField]
        private LayoutElement textLayoutElement;

        [SerializeField]
        private UserScrollDetector userScrollDetector;

        [SerializeField]
        private UserScrollBarTouchDetector userScrollBarTouchDetector;

        [SerializeField]
        internal UnityEvent onUserScrollingBegin = new();

        private void InnerOnUserScrollingBegin()
        {
            onUserScrollingBegin?.Invoke();
        }

        private void InnerScrollChanged(Vector2 v)
        {
            if (userScrollBarTouchDetector == null)
            {
                return;
            }

            if (userScrollBarTouchDetector.IsUserPressing)
            {
                InnerOnUserScrollingBegin();
            }
        }

        void Awake()
        {
            this.RunIfUnityObjectIsNotNull(userScrollDetector, usd =>
            {
                usd.onUserScrollBegin.AddListener(InnerOnUserScrollingBegin);
            });

            this.RunIfUnityObjectIsNotNull(scrollRect, sr =>
            {
                sr.onValueChanged.AddListener(InnerScrollChanged);
            });

            SetHorizontalTextWrap(wrapHorizontalText);
        }

        private async Task InnerScrollToAsync(float pos)
        {
            if (scrollRect == null)
            {
                return;
            }

            await Task.Yield();
            scrollRect.verticalNormalizedPosition = pos;
        }

        internal async Task ScrollToBottomAsync()
            => await InnerScrollToAsync(0f);

        internal void ScrollToBottom()
        {
            _ = ScrollToBottomAsync();
        }

        internal async Task ScrollToTopAsync()
            => await InnerScrollToAsync(1f);
        
        internal void ScrollToTop()
        {
            _ = ScrollToTopAsync();
        }

        internal void SetTitle(string title)
        {
            this.RunIfUnityObjectIsNotNull(titleTmpText, t => t.text = title);
        }


        internal void SetContentText(string text)
        {
            this.RunIfUnityObjectIsNotNull(textAreaTmpText, t => t.text = text);
        }

        private void SetHorizontalTextWrap(bool enabled)
        {
            if (contentSizeFitter == null || contentRectTransform == null)
            {
                return;
            }

            contentSizeFitter.horizontalFit = enabled
                ? ContentSizeFitter.FitMode.Unconstrained
                : ContentSizeFitter.FitMode.PreferredSize;

            var rtAnchorMax = contentRectTransform.anchorMax;
            rtAnchorMax.x = enabled ? 1f : 0f;
            contentRectTransform.anchorMax = rtAnchorMax;

            this.RunIfUnityObjectIsNotNull(textAreaTmpText, t =>
            {
#if UNITY_6000_0_OR_NEWER
                t.textWrappingMode = TextWrappingModes.Normal;
#else
                t.enableWordWrapping = enabled;
#endif
            });

            this.RunIfUnityObjectIsNotNull(textLayoutElement, le =>
            {
                le.minWidth = 0f;
                le.flexibleWidth = enabled ? 1f : 0f;
            });
        }
    }
}
