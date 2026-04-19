// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] Specifies the initialization state of a Unity Scene within the DemoKit environment.
    /// It uses a bitmask flag system to track whether a scene is valid, if it has been properly 
    /// set up with required components, and the user's preference for future setup prompts.
    /// </summary>
    [System.Flags]
    internal enum SceneInitializedStatus
    {
        SceneNotExists          = -1,
        NotInitialized          = 0,
        AlreadyInitialized      = 1,
        DoNotAskAgain           = 1 << 1,
    }
}