// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Threading;
using System.Threading.Tasks;
using Chobitech.DemoKit;
using UnityEngine;

public class MyAsyncDemo : AsyncDemoBase
{
    [SerializeField]
    private float durationSec = 2f;
    
    private Transform selfTransform;

    protected override void Awake()
    {
        base.Awake();

        // Caching the Transform of this object
        selfTransform = transform;
    }

    // Execute on demo canceled.
    public override void OnDemoCanceled(CancellationToken token)
    {
        AddLogLnWithColorTag("error", $"{IndividualDemoInfo.name} canceled by {token}");
    }

    // Execute when demo completed.
    public override void OnDemoCompleted()
    {
        AddLogLnWithColorTag("notice", $"{IndividualDemoInfo.name} finished.");
    }

    public override async Task DemoProcessAsync(CancellationToken token)
    {
        // Display the start log with "notice" color tag.
        AddLogLnWithColorTag("notice", $"Start {IndividualDemoInfo.name}");

        // Check if cancel is requested.
        token.ThrowIfCancellationRequested();


        /*
            Horizontal move
        */

        var initPos = selfTransform.localPosition;

        var elapsedSec = 0f;
        while (elapsedSec < durationSec)
        {
            token.ThrowIfCancellationRequested();

            var x = 2 * Mathf.Sin(2 * Mathf.PI * elapsedSec / durationSec);
            var pos = new Vector3(initPos.x + x, initPos.y, initPos.z);
            selfTransform.localPosition = pos;

            // Display the x of current position on the log area
            AddLogLn($"x = {x}");

            elapsedSec += Time.deltaTime;

            token.ThrowIfCancellationRequested();

            await Task.Yield();
        }

        // Reset position
        selfTransform.localPosition = initPos;

        token.ThrowIfCancellationRequested();
    }
}
