// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using UnityEngine;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A simple data structure to store the expansion state of a UI element.
    /// It pairs a unique identifier (key) with a boolean value to persist whether 
    /// a specific inspector foldout or UI section is currently expanded or collapsed.
    /// </summary>
    [System.Serializable]
    internal class ExpandableState
    {
        [SerializeField]
        internal string key;

        [SerializeField]
        internal bool expanded;
    }
}