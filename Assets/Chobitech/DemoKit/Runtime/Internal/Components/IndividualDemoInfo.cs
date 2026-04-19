// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEngine;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// A ScriptableObject that holds specific metadata for an individual demonstration.
    /// It stores the title and a detailed description to be displayed within the DemoKit UI system.
    /// Each <see cref="DemoBase"/> component requires a reference to this asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Chobitech/DemoKit/Individual Demo Information", fileName = "NewIndividualDemoInformation")]
    public class IndividualDemoInfo : ScriptableObject
    {
        /// <summary>
        /// Header title used for visual grouping within the Unity Inspector.
        /// </summary>
        [Header("Demo info")]

        /// <summary>
        /// The unique name or display title of the demonstration.
        /// This string is typically used as a header in the demo's UI panel.
        /// </summary>
        public string demoName;
        
        /// <summary>
        /// A comprehensive explanation of the demonstration's functionality and logic.
        /// The <see cref="TextAreaAttribute"/> provides an expanded editing area in the Inspector for better readability.
        /// </summary>
        [TextArea(5, 10)]
        public string description;
    }
}