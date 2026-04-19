// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A concrete implementation of <see cref="BaseStepLayout"/> for defining the demo's display name.
    /// It provides a simple text input interface that directly synchronizes with the 
    /// persistent <see cref="DemoSetupSettings"/>. The step is considered complete 
    /// once a non-empty string is provided.
    /// </summary>
    internal class DemoNameInputLayout : BaseStepLayout
    {
        private string _demoName;

        internal override string Title => "Input the name of your demo";

        internal override string Description => "";

        protected override bool? GetStepChecked()
        {
            return DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo => !string.IsNullOrEmpty(sInfo.demoName));
        }

        protected override bool InnerStepLayout()
        {
            return DemoSetupSettings.WithCurrentDemoSetupSettings(sInfo =>
            {
                _demoName = sInfo.demoName;

                return DemoKitGUI.TextField(
                    input =>
                    {
                        _demoName = input;
                        sInfo.demoName = _demoName;
                        DemoSetupSettings.Save();
                    },
                    _demoName
                );
            });
            
        }
    }
}