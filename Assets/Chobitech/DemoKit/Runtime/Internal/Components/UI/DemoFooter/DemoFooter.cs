// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// [Internal] Represents the footer UI component of the DemoKit.
    /// It manages the primary interaction controls, including a dropdown for selecting specific demos 
    /// and a run/stop button to control demo execution. Inherits from <see cref="RectTransformCachedMonoBehaviour"/> 
    /// for optimized layout positioning.
    /// </summary>
    internal class DemoFooter : RectTransformCachedMonoBehaviour
    {
        [Header("Events")]
        
        [SerializeField]
        internal UnityEvent<int> onDropDownSelectChanged;

        [SerializeField]
        internal UnityEvent<int> onRunButtonPressed;

        [Header("Components")]
        [SerializeField]
        private TMP_Dropdown dropDown;

        [SerializeField]
        private Button runButton;
        private TMP_Text _runButtonLabel;

        [SerializeField]
        private TMP_Text totalDemoCount;

        internal int SelectedDemoIndex => (dropDown != null && dropDown.interactable) ? dropDown.value : -1;

        private void OnDropDownValueChanged(int index)
        {
            onDropDownSelectChanged?.Invoke(index);
        }

        private void OnRunButtonClicked()
        {
            onRunButtonPressed?.Invoke(SelectedDemoIndex);
        }

        protected override void Awake()
        {
            base.Awake();

            this.RunIfUnityObjectIsNotNull(runButton, rb =>
            {
                rb.onClick.AddListener(OnRunButtonClicked);
                _runButtonLabel = rb.GetComponentInChildren<TMP_Text>();
            });

            this.RunIfUnityObjectIsNotNull(dropDown, dd =>
            {
                dd.onValueChanged.AddListener(OnDropDownValueChanged);
            });
        }


        internal void InitDropDownItems(IEnumerable<string> labels)
        {
            var labelArray = labels.ToArray();

            this.RunIfUnityObjectIsNotNull(dropDown, dd =>
            {
                dd.options.Clear();
                dd.value = 0;

                if (labelArray.Length > 0)
                {
                    dd.interactable = true;
                    dd.options = labelArray.Select(
                        label => new TMP_Dropdown.OptionData(label)
                    ).ToList();
                    dd.value = 0;
                }
                else
                {
                    dd.interactable = false;
                }
            });

            this.RunIfUnityObjectIsNotNull(totalDemoCount, ct =>
            {
                ct.text = $"Total Demos: {labelArray.Length}";
            });
        }

        internal void ChangeFooterState(bool isDemoRunning)
        {
            ChangeDropdownState(!isDemoRunning);
            this.RunIfUnityObjectIsNotNull(_runButtonLabel, label =>
            {
                label.text = isDemoRunning ? "Stop" : "Run";
            });
        }

        internal void ChangeDropdownState(bool enabled)
        {
            this.RunIfUnityObjectIsNotNull(dropDown, d =>
            {
                if (!enabled)
                {
                    d.Hide();
                }
                d.interactable = enabled;
            });
        }
    }
}
