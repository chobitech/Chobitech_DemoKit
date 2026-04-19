// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System;
using UnityEngine;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// A ScriptableObject that stores global configuration and metadata for the entire DemoKit system.
    /// It centralizes information like titles, icons, and shared rich text color tags used by all demo scenes.
    /// </summary>
    [CreateAssetMenu(menuName = "Chobitech/DemoKit/Global Demo Information", fileName = "NewGlobalDemoInformation")]
    public class GlobalDemoInfo : ScriptableObject
    {
        /// <summary>
        /// Header for basic identification details of the demonstration kit.
        /// </summary>
        [Header("Global Demo Basic Info")]
        
        /// <summary>
        /// The main title of the entire demonstration kit, typically displayed in the UI header.
        /// </summary>
        public string title;

        /// <summary>
        /// The icon or logo representing the global demonstration, used for branding in the UI.
        /// </summary>
        public Sprite icon;

        /// <summary>
        /// A comprehensive description for the entire demonstration system, explaining its purpose or usage.
        /// </summary>
        [TextArea(5, 10)]
        public string globalDescription;

        /// <summary>
        /// Header for internal settings and formatting configurations.
        /// </summary>
        [Header("Other Settings")]

        /// <summary>
        /// A collection of <see cref="CustomColorTag"/> assets that define shared color formatting for log messages.
        /// </summary>
        public CustomColorTag[] customColorTags;

        [NonSerialized]
        private CustomColorTagHolder _holder = new();

        /// <summary>
        /// Internal accessor for the <see cref="CustomColorTagHolder"/> which manages the tag-to-color mapping.
        /// </summary>
        internal CustomColorTagHolder CustomColorTagHolder => _holder;

        private void InitColorTagMap(bool forceReset)
        {
            if (forceReset)
            {
                _holder = null;
            }

            if (_holder != null)
            {
                return;
            }

            _holder = new(customColorTags);
        }

        /// <summary>
        /// Initializes or resets the color tag map when the ScriptableObject is loaded or enabled in the editor.
        /// </summary>
        void OnEnable()
        {
            InitColorTagMap(true);
        }
    }
}
