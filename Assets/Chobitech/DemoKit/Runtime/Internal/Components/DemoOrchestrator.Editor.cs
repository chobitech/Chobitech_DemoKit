// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


#if UNITY_EDITOR

using UnityEditor.SceneManagement;
using UnityEngine;

namespace Chobitech.DemoKit
{
    public partial class DemoOrchestrator : MonoBehaviour
    {
        void OnValidate()
        {
            if (!DemoKitUtils.IsPlaying)
            {
                var scene = gameObject.scene;
                var flag = scene.IsValid() && scene.isLoaded && scene == EditorSceneManager.GetActiveScene();
                InitDemo(flag);
            }
        }
    }
}

#endif
