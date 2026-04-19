// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// [Internal] Detects touch or click interactions specifically for a scroll bar to track its active state.
    /// Implements <see cref="IPointerDownHandler"/> and <see cref="IPointerUpHandler"/> to monitor when a user is 
    /// holding the scroll bar, typically used to pause auto-scrolling during manual navigation.
    /// </summary>
    internal class UserScrollBarTouchDetector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        internal bool IsUserPressing { get; private set; } = false;

        internal UnityEvent<bool> onScrollBarPressingStateChanged;

        public void OnPointerDown(PointerEventData eventData)
        {
            IsUserPressing = true;
            onScrollBarPressingStateChanged?.Invoke(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsUserPressing = false;
            onScrollBarPressingStateChanged?.Invoke(false);
        }
    }
}
