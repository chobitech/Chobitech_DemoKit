// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// [Internal] Detects user-initiated scrolling or dragging within a UI area to intercept auto-scroll behavior.
    /// This component implements <see cref="IBeginDragHandler"/> and <see cref="IScrollHandler"/> to trigger events 
    /// when manual navigation is detected, typically used to disable auto-scrolling in the log area.
    /// </summary>
    internal class UserScrollDetector : MonoBehaviour, IBeginDragHandler, IScrollHandler
    {
        [SerializeField]
        internal UnityEvent onUserScrollBegin;

        public void OnBeginDrag(PointerEventData eventData)
        {
            onUserScrollBegin?.Invoke();
        }

        public void OnScroll(PointerEventData eventData)
        {
            onUserScrollBegin?.Invoke();
        }
    }
}
