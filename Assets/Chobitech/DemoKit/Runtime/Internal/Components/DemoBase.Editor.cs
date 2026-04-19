// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


#if UNITY_EDITOR

using UnityEngine;

namespace Chobitech.DemoKit
{
    public abstract partial class DemoBase : MonoBehaviour
    {
        void OnValidate()
        {
            if (!DemoKitUtils.IsPlaying)
            {
                CheckIndividualDemoInfo();
            }
        }
    }
}

#endif

