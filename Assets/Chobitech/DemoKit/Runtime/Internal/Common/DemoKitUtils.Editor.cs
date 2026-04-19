// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace Chobitech.DemoKit
{
    public static partial class DemoKitUtils
    {
        /// <summary>
        /// Gets a value indicating whether the application is currently in play mode or transitioning to it.
        /// In the Unity Editor, this also returns true if the play mode is about to start.
        /// </summary>
        public static bool IsPlaying
            => Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode;
    }
}

#endif
