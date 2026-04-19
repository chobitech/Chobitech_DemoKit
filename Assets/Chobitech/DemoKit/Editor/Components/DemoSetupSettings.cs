// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using System;
using System.Collections.Generic;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A persistent singleton that manages a collection of <see cref="DemoSetupInfo"/>.
    /// It stores various demo configurations within the ProjectSettings folder and provides 
    /// a centralized interface to switch, add, or modify the currently active demo setup.
    /// It ensures data persistence across Editor sessions using <see cref="ScriptableSingleton{T}"/>.
    /// </summary>
    [FilePath("ProjectSettings/DemoSetupSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    internal class DemoSetupSettings : ScriptableSingleton<DemoSetupSettings>
    {

        [SerializeField]
        private List<DemoSetupInfo> demoSetupInfoList = new();

        internal static DemoSetupInfo[] DemoSetupInfoArray => instance.demoSetupInfoList.ToArray();


        [SerializeField]
        private int currentActiveInfoIndex = 0;

        internal static int CurrentActiveInfoIndex => instance.currentActiveInfoIndex;
        
        private DemoSetupInfo GetDemoSetupInfo(int index)
        {
            var count = demoSetupInfoList.Count;
            if (count > 0)
            {
                if (index >= count)
                {
                    DemoKitLog.Exception(new IndexOutOfRangeException());
                    return null;
                }

                return demoSetupInfoList[index];
            }
            else
            {
                return AddNewDemoSetupInfo();
            }
        }

        private bool InnerClearSetupInfo(int index)
        {
            var count = demoSetupInfoList.Count;
            if (count > 0 && index < count)
            {
                demoSetupInfoList[index] = new();
                return true;
            }
            return false;
        }

        internal static DemoSetupInfo CurrentDemoSetupInfo
            => instance.GetDemoSetupInfo(instance.currentActiveInfoIndex);
        
        internal static bool ClearSetupInfo(int index)
            => instance.InnerClearSetupInfo(index);
        
        internal static bool ClearCurrentSetupInfo()
            => instance.InnerClearSetupInfo(instance.currentActiveInfoIndex);


        internal static void ChangeCurrentSetupInfo(int index)
        {
            var demoInfo = instance.GetDemoSetupInfo(index);
            if (demoInfo != null)
            {
                instance.currentActiveInfoIndex = index;
            }
        }

        internal static DemoSetupInfo AddNewDemoSetupInfo()
        {
            var demoInfo = new DemoSetupInfo();
            instance.currentActiveInfoIndex = instance.demoSetupInfoList.Count;
            instance.demoSetupInfoList.Add(demoInfo);
            return demoInfo;
        }

        internal void SaveInstance() => Save(true);

        internal static void Save() => instance.SaveInstance();

        

        public static T WithCurrentDemoSetupSettings<T>(System.Func<DemoSetupInfo, T> func, bool saveOnExit = false)
        {
            try
            {
                if (func != null)
                {
                    return func(CurrentDemoSetupInfo);
                }
                return default;
            }
            finally
            {
                if (saveOnExit)
                {
                    Save();
                }
            }
        }

        public static void WithCurrentDemoSetupSettings(UnityAction<DemoSetupInfo> action, bool saveOnExit = false)
            => WithCurrentDemoSetupSettings<bool>(
                cur =>
                {
                    action?.Invoke(cur);
                    return false;
                },
                saveOnExit
            );
    }
}