// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] Registers and manages the DemoKit settings within the Unity Project Settings window,
    /// providing a centralized location for demo configuration alongside standard project categories.
    /// </summary>
    internal class DemoKitProjectSetting : SettingsProvider
    {
        internal DemoKitProjectSetting(string path, SettingsScope scopes = SettingsScope.Project, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        private DemoKitGUI.PaddingAndScroll _scroll;
        private SetupMainLayout _setupMainLayout;

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            _scroll = new();
            _setupMainLayout = new();

            EditorApplication.projectChanged -= OnProjectChanged;
            EditorApplication.projectChanged += OnProjectChanged;
        }

        public override void OnDeactivate()
        {
            EditorApplication.projectChanged -= OnProjectChanged;
        }

        private void OnProjectChanged()
        {
            TMPImportedChecker.Reset();
            Repaint();
        }



    

        public override void OnGUI(string searchContext)
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
                if (_setupMainLayout.ShowLayout())
                {
                    Repaint();
                }
            });
        }

        [SettingsProvider]
        internal static SettingsProvider CreateDemoKitSettingsProvider()
        {
            return new DemoKitProjectSetting("Project/Chobitech/DemoKit Setup", SettingsScope.Project)
            {
                keywords = new HashSet<string>(new[] { "Demo", "DemoKit", "chobitech", "Setup" })
            };
        }
    }
}
