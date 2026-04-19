// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

namespace Chobitech.DemoKit
{
    /// <summary>
    /// [Internal] Base class for manager components that handle the execution lifecycle of a <see cref="DemoBase"/>.
    /// This abstraction allows the orchestrator to manage both Task-based and Coroutine-based demos through a unified interface.
    /// </summary>
    internal abstract class DemoManagerBase
    {
        internal abstract DemoBase DemoBase { get; }

        internal abstract void StartDemo(params object[] args);

        internal abstract void CancelDemo();

        internal abstract bool IsDemoRunning { get; }
    }
}