// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;


namespace Chobitech.DemoKit
{
    /// <summary>
    /// Base class for all demonstration components.
    /// Provides common functionality for metadata management, logging, and lifecycle hooks.
    /// This class is inherited by <see cref="AsyncDemoBase"/> and <see cref="CoroutineDemoBase"/>.
    /// </summary>
    public abstract partial class DemoBase : MonoBehaviour
    {
        /// <summary>
        /// Internal reference to the type of DemoBase for quick reflection or identification.
        /// </summary>
        internal static readonly Type SelfType = typeof(DemoBase);

        [Header("Demo Information")]
        [SerializeField]
        private IndividualDemoInfo individualDemoInfo;

        /// <summary>
        /// Gets the metadata associated with this specific demonstration.
        /// </summary>
        public IndividualDemoInfo IndividualDemoInfo => individualDemoInfo;

        /// <summary>
        /// Gets the name of the demonstration defined in <see cref="IndividualDemoInfo"/>.
        /// Returns a default name if the metadata is missing.
        /// </summary>
        public string DemoName => (individualDemoInfo != null) ? individualDemoInfo.demoName : $"No Name Demo";

        /// <summary>
        /// Gets the description text of the demonstration.
        /// </summary>
        public string Description => (individualDemoInfo != null) ? individualDemoInfo.description : "";

        /// <summary>
        /// Gets or sets the orchestrator managing this demo.
        /// This is typically assigned by <see cref="DemoOrchestrator"/> during the initialization phase.
        /// </summary>
        internal DemoOrchestrator Orchestrator { get; set; }

        /// <summary>
        /// Called when the demonstration process completes successfully without being canceled.
        /// Override this to perform specific actions upon successful demo completion.
        /// </summary>
        public virtual void OnDemoCompleted()
        {
            
        }

        private bool CheckIndividualDemoInfo()
        {
            if (individualDemoInfo == null)
            {
                DemoKitLog.Warning($"The individual demo info is not set on {name}.", this);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Initializes the demo component and performs a validation check on the metadata.
        /// </summary>
        protected virtual void Awake()
        {
            CheckIndividualDemoInfo();
        }

        private T WithParentDemoOrchestrator<T>(Func<DemoOrchestrator, T> func)
        {
            return this.RunIfUnityObjectIsNotNull(Orchestrator, dm =>
            {
                if (func != null)
                {
                    return func(dm);
                }
                return default;
            });
        }

        private void WithParentDemoOrchestrator(UnityAction<DemoOrchestrator> action)
        {
            _ = WithParentDemoOrchestrator<bool>(dm =>
            {
                action?.Invoke(dm);
                return false;
            });
        }

        private T WithParentGlobalDemoInfo<T>(Func<GlobalDemoInfo, T> func)
        {
            return WithParentDemoOrchestrator(dm =>
            {
                return dm.WithGlobalDemoInfo(func);
            });
        }

        //--- logs
        private readonly StringBuilder _logSb = new();

        /// <summary>
        /// Gets the full log string currently stored in the internal buffer for this demo.
        /// </summary>
        public string CurrentLog => _logSb.ToString();

        /// <summary>
        /// Sends the current content of the internal log buffer to the UI display via the orchestrator.
        /// </summary>
        public void ApplyStoredLog()
        {
            WithParentDemoOrchestrator(dm => dm.SetLogText(_logSb.ToString()));
        }

        /// <summary>
        /// Clears the internal log buffer and updates the UI display to reflect the empty state.
        /// </summary>
        public void ClearLog()
        {
            _logSb.Clear();
            ApplyStoredLog();
        }

        private void InnerAddLog(string tag, string log, bool withNewLine)
        {
            if (tag != null)
            {
                log = CustomTagController.GetCustomTaggedText(tag, log);
            }

            log = WithParentGlobalDemoInfo(dmi => dmi.CustomColorTagHolder.ConvertCustomTagToRichText(log));
            
            _logSb.Append(log);

            if (withNewLine)
            {
                _logSb.Append("\n");
            }

            ApplyStoredLog();
        }

        /// <summary>
        /// Appends a message to the internal log buffer and updates the UI.
        /// </summary>
        /// <param name="log">The text message to add to the log.</param>
        public void AddLog(string log)  
            => InnerAddLog(null, log, false);

        /// <summary>
        /// Appends a message followed by a newline character to the internal log buffer and updates the UI.
        /// </summary>
        /// <param name="log">The text message to add to the log.</param>
        public void AddLogLn(string log)
            => InnerAddLog(null, log, true);
        
        /// <summary>
        /// Appends a message wrapped in a custom color tag to the log buffer.
        /// Custom tags are defined in the <see cref="GlobalDemoInfo"/> asset.
        /// </summary>
        /// <param name="tag">The identifier of the custom tag.</param>
        /// <param name="log">The text message to add to the log.</param>
        public void AddLogWithColorTag(string tag, string log)  
            => InnerAddLog(tag, log, false);
        
        /// <summary>
        /// Appends a message wrapped in a custom color tag, followed by a newline, to the log buffer.
        /// </summary>
        /// <param name="tag">The identifier of the custom tag.</param>
        /// <param name="log">The text message to add to the log.</param>
        public void AddLogLnWithColorTag(string tag, string log)
            => InnerAddLog(tag, log, true);
    }
}
