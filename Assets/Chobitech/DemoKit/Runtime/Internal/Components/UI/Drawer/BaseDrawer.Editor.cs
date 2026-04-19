// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php



#if UNITY_EDITOR

using UnityEngine;

namespace Chobitech.DemoKit
{
    internal partial class BaseDrawer : MonoBehaviour
    {
        protected virtual void OnValidate()
        {
            if (!DemoKitUtils.IsPlaying)
            {
                InitDrawerArea();
            }
        }
    }
}

#endif

