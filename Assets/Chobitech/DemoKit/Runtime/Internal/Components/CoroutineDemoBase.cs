// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using System.Collections;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// Base class for demonstrations using Unity Coroutines (IEnumerator).
    /// Suitable for tasks that are tightly integrated with the Unity lifecycle or for users 
    /// who prefer the standard StartCoroutine pattern.
    /// Inherits from <see cref="DemoBase"/> to provide metadata and logging functionality.
    /// </summary>
    public abstract class CoroutineDemoBase : DemoBase
    {
        /// <summary>
        /// Defines the main execution logic of the coroutine-based demonstration.
        /// This method is started and managed by the <see cref="DemoOrchestrator"/> using the Unity Coroutine system.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> used by the Unity Coroutine engine.</returns>
        public abstract IEnumerator DemoRoutine();

        /// <summary>
        /// Triggered when the coroutine demonstration is forcibly stopped (canceled) by the orchestrator.
        /// Override this method to handle specific cleanup, UI resets, or state restoration for canceled coroutines.
        /// </summary>
        public virtual void OnDemoCanceled()
        {
            
        }
    }
}
