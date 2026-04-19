// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEngine;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// [Internal] A base class for UI-related <see cref="MonoBehaviour"/> components that frequently access their <see cref="RectTransform"/>.
    /// It provides a cached reference to the RectTransform and its size property to optimize performance and simplify UI layout logic.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    internal class RectTransformCachedMonoBehaviour : MonoBehaviour
    {
        internal RectTransform SelfRectTransform { get; private set; }

        internal Vector2 Size => SelfRectTransform.rect.size;

        protected virtual void Awake()
        {
            SelfRectTransform = GetComponent<RectTransform>();
        }
    }
}