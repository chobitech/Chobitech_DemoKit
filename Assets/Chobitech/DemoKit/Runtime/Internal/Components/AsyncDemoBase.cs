// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using System.Threading;
using System.Threading.Tasks;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// Base class for asynchronous demonstrations using Task and CancellationToken.
    /// Ideal for complex asynchronous logic, awaiting multiple operations, or integrating with Task-based APIs.
    /// This class inherits from <see cref="DemoBase"/> to provide metadata and logging functionality.
    /// </summary>
    public abstract class AsyncDemoBase : DemoBase
    {
        /// <summary>
        /// Defines the main execution logic of the asynchronous demonstration.
        /// This method is invoked by the <see cref="DemoOrchestrator"/>. 
        /// Implementations should respect the provided <see cref="CancellationToken"/> to ensure responsive cancellation.
        /// </summary>
        /// <param name="token">A token used to monitor for cancellation requests from the orchestrator or system.</param>
        /// <returns>A <see cref="Task"/> representing the ongoing asynchronous operation.</returns>
        public abstract Task DemoProcessAsync(CancellationToken token);

        /// <summary>
        /// Triggered when the asynchronous demonstration process is canceled via the <see cref="CancellationToken"/>.
        /// Override this method to perform specific cleanup, UI resets, or custom logging upon cancellation.
        /// </summary>
        /// <param name="token">The cancellation token that triggered this state.</param>
        public virtual void OnDemoCanceled(CancellationToken token)
        {
            DemoKitLog.Info($"{DemoName} was canceled.");
        }
    }
}
