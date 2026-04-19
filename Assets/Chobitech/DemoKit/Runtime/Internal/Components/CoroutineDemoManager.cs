// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Chobitech.DemoKit
{

    /// <summary>
    /// [Internal] Concrete implementation of <see cref="DemoManagerBase"/> for managing coroutine-based demonstrations.
    /// Handles the lifecycle of Unity Coroutines, including starting, stopping, and triggering completion/cancellation hooks.
    /// </summary>
    internal class CoroutineDemoManager : DemoManagerBase
    {
        internal readonly CoroutineDemoBase coroutineDemoBase;

        internal readonly MonoBehaviour parentMb;
        
        private readonly UnityAction _onDemoFinished;

        private Coroutine _demoCoroutine;

        internal CoroutineDemoManager(CoroutineDemoBase coroutineDemoBase, MonoBehaviour parentMb, UnityAction onDemoFinished = null)
        {
            this.coroutineDemoBase = coroutineDemoBase;
            this.parentMb = parentMb;
            _onDemoFinished = onDemoFinished;
        }

        private void StopDemoCoroutine()
        {
            if (parentMb != null && parentMb.isActiveAndEnabled && _demoCoroutine != null)
            {
                parentMb.StopCoroutine(_demoCoroutine);
                _demoCoroutine = null;
            }
        }

        private IEnumerator InnerDemoProcessRoutine()
        {
            // Execute the main demo routine
            yield return coroutineDemoBase.DemoRoutine();

            _demoCoroutine = null;
            
            // Call hook on successful completion
            coroutineDemoBase.OnDemoCompleted();

            // Notify the orchestrator
            _onDemoFinished?.Invoke();
        }


        internal override DemoBase DemoBase => coroutineDemoBase;

        internal override bool IsDemoRunning => _demoCoroutine != null;

        internal override void StartDemo(params object[] args)
        {
            StopDemoCoroutine();

            if (coroutineDemoBase == null)
            {
                DemoKitLog.Warning($"Coroutine Demo Start Failed: \"{nameof(coroutineDemoBase)}\" is null");
                return;
            }

            if (parentMb == null)
            {
                DemoKitLog.Warning($"Coroutine Demo Start Failed: \"{nameof(parentMb)}\" is null");
                return;
            }

            _demoCoroutine = parentMb.StartCoroutine(InnerDemoProcessRoutine());
        }

        internal override void CancelDemo()
        {
            StopDemoCoroutine();
            // Call hook specifically for cancellation
            this.RunIfUnityObjectIsNotNull(coroutineDemoBase, d => d.OnDemoCanceled());
            _onDemoFinished?.Invoke();
        }
    }
}