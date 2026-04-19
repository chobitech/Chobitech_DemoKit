// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEditor;

// TODO: check output logs in each process

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] The main editor window for Chobitech.DemoKit. 
    /// It serves as a central hub for the entire demo lifecycle, integrating the 
    /// initial environment setup, implementation guide, and distribution file generation. 
    /// The window is designed to be docked near the Inspector for ease of access and 
    /// utilizes a scrollable, section-based layout to provide a seamless user experience.
    /// </summary>
    internal class DemoKitSetupWindow : EditorWindow
    {
        [MenuItem(DemoKitEditorMenu.SetupWindowPath, false, 60000)]
        internal static void ShowSetupWindow()
        {
            var inspectorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow");
            
            var window = GetWindow<DemoKitSetupWindow>(
                DemoKitEditorMenu.SetupWindowName,
                false,
                inspectorType
            );

            window.Show(true);
        }

        private DemoKitGUI.PaddingAndScroll _scroll;

        private SetupMainLayout _setupMainLayout;


        void OnEnable()
        {
            _scroll = new();
            _setupMainLayout = new();
        }

        private void OnGUI()
        {
            if (_scroll == null)
            {
                _scroll = new();
            }

            if (_setupMainLayout == null)
            {
                _setupMainLayout = new();
            }
            
            _scroll.Show(() =>
                {
                    HeaderLayout.ShowLayout();

                    var needsRepaint = _setupMainLayout.ShowLayout();

                    if (needsRepaint)
                    {
                        Repaint();
                    }
                }
            );
        }

        void OnProjectChange()
        {
            TMPImportedChecker.Reset();
            Repaint();
        }
    }
}