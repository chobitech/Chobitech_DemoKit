// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// The central controller for the DemoKit system.
    /// It manages the lifecycle of demonstrations, coordinates between UI components (Header, Footer, Log, Description),
    /// and handles asynchronous execution and cancellation of demo processes.
    /// </summary>
    public partial class DemoOrchestrator : MonoBehaviour
    {
        /// <summary>
        /// Reference to the global configuration asset containing kit-wide metadata and color tags.
        /// </summary>
        [Header("Global Demo Information")]
        public GlobalDemoInfo globalDemoInfo;

        /// <summary>
        /// The Transform container that holds all individual demo GameObjects.
        /// The orchestrator scans this container to initialize demo managers.
        /// </summary>
        [Header("Demo Object Container")]
        [SerializeField]
        private Transform demoContainer;

        [Header("UI States and Values")]
        [SerializeField]
        private bool openDescriptionAreaAtStart = false;

        [SerializeField]
        private bool openLogAreaAtStart = false;

        [Header("Other Components and Assets")]
        [SerializeField]
        private DemoHeader header;

        [SerializeField]
        private DemoFooter footer;

        [SerializeField]
        private DemoDescriptionArea descriptionArea;

        [SerializeField]
        private DemoLogArea logArea;

        [NonSerialized]
        private List<DemoManagerBase> _demoManagers;

        private DemoManagerBase _currentDemoManager;

        private void InnerOnDemoFinished()
        {
            this.RunIfUnityObjectIsNotNull(footer, f => f.ChangeFooterState(false));
        }

        /// <summary>
        /// Requests the cancellation of the currently running demonstration.
        /// Also updates the footer UI state to reflect the stopped status.
        /// </summary>
        internal void CancelDemo()
        {
            _currentDemoManager?.CancelDemo();
            this.RunIfUnityObjectIsNotNull(footer, f => f.ChangeFooterState(false));
        }

        /// <summary>
        /// Starts the currently selected demonstration.
        /// If another demo is running, it will be canceled first.
        /// Passes the <see cref="MonoBehaviour.destroyCancellationToken"/> to the demo manager for safety.
        /// </summary>
        internal void StartDemo()
        {
            CancelDemo();

            if (_currentDemoManager == null)
            {
                return;
            }

            this.RunIfUnityObjectIsNotNull(footer, f => f.ChangeFooterState(true));

            _currentDemoManager.StartDemo(destroyCancellationToken);
        }

        private void StartAndStopDemo(int index)
        {
            if (_currentDemoManager == null)
            {
                return;
            }

            if (_currentDemoManager.IsDemoRunning)
            {
                CancelDemo();
            }
            else
            {
                StartDemo();
            }
        }

        /// <summary>
        /// Converts custom pseudo-tags within the text into Unity rich text format using global color settings.
        /// </summary>
        /// <param name="text">The raw text containing custom tags.</param>
        /// <returns>The processed text with rich text color tags applied.</returns>
        internal string GetTagConvertedText(string text)
        {
            return WithGlobalDemoInfo(gdi =>
            {
                return gdi.CustomColorTagHolder.ConvertCustomTagToRichText(text);
            }) ?? text;
        }
        
        private void OnDemoSelectChanged(int index)
        {
            DemoBase demo = null;

            for (var i = 0; i < _demoManagers.Count; i++)
            {
                var manager = _demoManagers[i];
                var hit = i == index;
                if (hit)
                {
                    _currentDemoManager = manager;
                    demo = manager.DemoBase;
                }
                manager.DemoBase.gameObject.SetActive(hit);
            }

            if (demo == null)
            {
                return;
            }

            this.RunIfUnityObjectIsNotNull(descriptionArea, da =>
            {
                da.SetCurrentDemoTitle(demo.DemoName);
                da.SetCurrentDemoDescription(GetTagConvertedText(demo.Description));
            });

            this.RunIfUnityObjectIsNotNull(logArea, la =>
            {
                la.SetLogText(demo.CurrentLog);
                la.EnableAutoScroll();
            });
        }

        private void LoadDemos(bool showLogs)
        {
            if (_demoManagers != null)
            {
                return;
            }

            _demoManagers = new();
            
            this.RunIfUnityObjectIsNotNull(demoContainer, container =>
            {
                foreach (Transform childTr in container)
                {
                    if (childTr.TryGetComponent<DemoBase>(out var demo))
                    {
                        demo.Orchestrator = this;
                        if (demo is AsyncDemoBase a)
                        {
                            _demoManagers.Add(new AsyncDemoManager(a, InnerOnDemoFinished));
                        }
                        else if (demo is CoroutineDemoBase c)
                        {
                            _demoManagers.Add(new CoroutineDemoManager(c, this, InnerOnDemoFinished));
                        }
                    }
                }
            });

            this.RunIfUnityObjectIsNotNull(footer, f =>
            {
                if (_demoManagers.Count > 0)
                {
                    var items = _demoManagers.Select(d => d.DemoBase.DemoName);
                    f.InitDropDownItems(items);
                    OnDemoSelectChanged(0);
                }
                else
                {
                    f.InitDropDownItems(Array.Empty<string>());
                    UnityEngine.Object context = (demoContainer != null) ? demoContainer : this;
                    if (showLogs)
                    {
                        DemoKitLog.Warning($"DemoBase not exist. Put DemoBase into the demo container.", context);
                    }
                }
            });
        }

        private void ApplyDemoInformation(GlobalDemoInfo demoInfo, bool showErrorLog)
        {
            if (demoInfo == null)
            {
                if (showErrorLog)
                {
                    DemoKitLog.Warning($"The {nameof(GlobalDemoInfo)} is not attached to {nameof(DemoOrchestrator)}.", this);
                }
                return;
            }
            
            this.RunIfUnityObjectIsNotNull(header, h =>
            {
                h.SetIcon(demoInfo.icon);
                h.SetTitle(demoInfo.title);
            });

            this.RunIfUnityObjectIsNotNull(descriptionArea, da =>
            {
                da.SetGlobalDescription(GetTagConvertedText(demoInfo.globalDescription));
                da.SetCurrentDemoDescription("");
            });
        }

        private void CheckEventSystem(bool showLogs)
        {
            var scene = gameObject.scene;

            if (!scene.IsValid() || !scene.isLoaded)
            {
                return;
            }

            var esObj = scene.GetRootGameObjects()
                .SelectMany(obj => obj.GetComponentsInChildren<EventSystem>(true))
                .FirstOrDefault();
            
            if (esObj == null && showLogs)
            {
                DemoKitLog.Warning($"No EventSystem found in DemoMainScene. UI interactions are disabled. Please add one via [GameObject] > [UI] > [EventSystem].", this);
            }
        }

        private void InitDemo(bool showLogs)
        {
            ApplyDemoInformation(globalDemoInfo, showLogs);
            CheckEventSystem(showLogs);
            LoadDemos(showLogs);

            this.RunIfUnityObjectIsNotNull(footer, f =>
            {
                f.onRunButtonPressed.AddListener(StartAndStopDemo);
                f.onDropDownSelectChanged.AddListener(OnDemoSelectChanged);
            });
        }

        /// <summary>
        /// Executes a function that requires the <see cref="GlobalDemoInfo"/>.
        /// Safely handles null checks and returns the result.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <param name="func">The function to execute if the global info is available.</param>
        /// <returns>The result of the function or the default value of T.</returns>
        internal T WithGlobalDemoInfo<T>(Func<GlobalDemoInfo, T> func)
        {
            return this.RunIfUnityObjectIsNotNull(globalDemoInfo, dmi =>
            {
                if (func != null)
                {
                    return func(dmi);
                }
                return default;
            });
        }

        /// <summary>
        /// Executes an action that requires the <see cref="GlobalDemoInfo"/>.
        /// Safely handles null checks before invocation.
        /// </summary>
        /// <param name="action">The action to execute if the global info is available.</param>
        internal void WithGlobalDemoInfo(UnityAction<GlobalDemoInfo> action)
        {
            _ = WithGlobalDemoInfo<bool>(dmi =>
            {
                action?.Invoke(dmi);
                return false;
            });
        }

        /// <summary>
        /// Updates the content displayed in the UI log area.
        /// </summary>
        /// <param name="logText">The text to display in the log.</param>
        internal void SetLogText(string logText)
        {
            this.RunIfUnityObjectIsNotNull(logArea, la => la.SetLogText(logText));
        }

        void Awake()
        {
            InitDemo(true);
        }

        private void SetTopAndBottomMargin()
        {
            var top = this.RunIfUnityObjectIsNotNull(header, h => h.Size.y, 0f);
            var bottom = this.RunIfUnityObjectIsNotNull(footer, f => f.Size.y, 0f);
            this.RunIfUnityObjectIsNotNull(descriptionArea, da => da.BaseDrawer.SetMargin(top, bottom));
            this.RunIfUnityObjectIsNotNull(logArea, la => la.BaseDrawer.SetMargin(top, bottom));
        }

        async Task Start()
        {
            Canvas.ForceUpdateCanvases();

            await Task.Yield();

            SetTopAndBottomMargin();

            this.RunIfUnityObjectIsNotNull(descriptionArea, da =>
            {
                if (openDescriptionAreaAtStart)
                {
                    da.BaseDrawer.OpenDrawer(false);
                }
                else
                {
                    da.BaseDrawer.CloseDrawer(false);
                }
            });

            this.RunIfUnityObjectIsNotNull(logArea, la =>
            {
                if (openLogAreaAtStart)
                {
                    la.BaseDrawer.OpenDrawer(false);
                }
                else
                {
                    la.BaseDrawer.CloseDrawer(false);
                }
            });
        }
    }
}