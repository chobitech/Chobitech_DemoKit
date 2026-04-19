// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Chobitech.DemoKit
{

    /// <summary>
    /// [Internal] Concrete implementation of <see cref="DemoManagerBase"/> for managing Task-based asynchronous demonstrations.
    /// Handles CancellationToken lifecycle, linked tokens from external sources, and orchestration of async execution flows
    /// using a thread-safe semaphore to prevent race conditions during initialization and disposal.
    /// </summary>
    internal class AsyncDemoManager : DemoManagerBase
    {
        internal const int lockWaitTimeoutMs = 1000;

        internal readonly AsyncDemoBase asyncDemo;

        private readonly UnityAction _onDemoFinished;

        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private CancellationTokenSource _cts;

        internal AsyncDemoManager(AsyncDemoBase asyncDemo, UnityAction onDemoFinished = null)
        {
            this.asyncDemo = asyncDemo;
            _onDemoFinished = onDemoFinished;
        }

        private void DisposeCts()
        {
            if (_semaphore.Wait(lockWaitTimeoutMs))
            {
                try
                {
                    _cts?.Cancel();
                    _cts?.Dispose();
                    _cts = null;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        internal override DemoBase DemoBase => asyncDemo;

        internal override bool IsDemoRunning => _cts?.IsCancellationRequested == false;

        private async Task InnerStartDemoAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await asyncDemo.DemoProcessAsync(token);
                token.ThrowIfCancellationRequested();

                // Call hook on successful completion
                asyncDemo.OnDemoCompleted();
            }
            catch (OperationCanceledException ex)
            {
                // Call hook on cancellation
                asyncDemo.OnDemoCanceled(ex.CancellationToken);
            }
            finally
            {
                // Notify the orchestrator that the demo has finished
                _onDemoFinished?.Invoke();
            }
        }

        internal override void StartDemo(params object[] args)
        {
            DisposeCts();

            if (asyncDemo == null)
            {
                DemoKitLog.Warning($"Demo Start Failed: \"{nameof(asyncDemo)}\" is null");
                return;
            }

            var cTokenList = new List<CancellationToken>();
            foreach (var a in args)
            {
                if (a is CancellationToken t)
                {
                    cTokenList.Add(t);
                }
            }

            if (_semaphore.Wait(lockWaitTimeoutMs))
            {
                try
                {
                    _cts = CancellationTokenSource.CreateLinkedTokenSource(cTokenList.ToArray());
                }
                finally
                {
                    _semaphore.Release();
                }

                _ = InnerStartDemoAsync(_cts.Token);
            }
        }

        internal override void CancelDemo()
        {
            DisposeCts();
        }
    }
}