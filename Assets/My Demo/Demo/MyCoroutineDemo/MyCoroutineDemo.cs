// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Collections;
using Chobitech.DemoKit;
using UnityEngine;

public class MyCoroutineDemo : CoroutineDemoBase
{
    private Transform selfTransform;

    [SerializeField]
    private float durationSec = 2f;

    protected override void Awake()
    {
        base.Awake();

        // Caching the Transform of this object
        selfTransform = transform;
    }

    // Execute on demo canceled.
    public override void OnDemoCanceled()
    {
        AddLogLnWithColorTag("error", $"{IndividualDemoInfo.name} is canceled.");
    }

    // Execute on demo completed.
    public override void OnDemoCompleted()
    {
        AddLogLnWithColorTag("warning", $"{IndividualDemoInfo.name} finished.");
    }

    public override IEnumerator DemoRoutine()
    {
        // Display the start log with "notice" color tag.
        AddLogLnWithColorTag("warning", $"Start {IndividualDemoInfo.name}");

        /*
            Vertical move
        */

        var initPos = selfTransform.localPosition;
        
        var elapsedSec = 0f;
        while (elapsedSec < durationSec)
        {
            var y = 2 * Mathf.Sin(2 * Mathf.PI * elapsedSec / durationSec);
            var pos = new Vector3(initPos.x, initPos.y + y, initPos.z);

            selfTransform.localPosition = pos;

            // Display the y of current position on the log area
            AddLogLn($"y = {y}");

            elapsedSec += Time.deltaTime;

            yield return null;
        }

        // Reset position
        selfTransform.localPosition = initPos;
    }
}
