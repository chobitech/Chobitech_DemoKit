// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using UnityEditor;
using UnityEngine;

namespace Chobitech.DemoKit.Editor
{
    internal class CheckRequiredAssetsLayout : BaseSectionLayout
    {
        private static readonly Color bgColor = new(1f, 0.3f, 0.1f, 0.25f);

        internal override string Title => "Import Required Assets";

        internal override string Description => null;

        internal override Color? BgColor => bgColor;

        private bool TmpImportLayout()
        {
            var needsRepaint = false;

            var isTmpImported = TMPImportedChecker.IsImported;

            DemoKitGUI.WithCheckMark(
                isTmpImported,
                () =>
                {
                    DemoKitGUI.SmallHeading($"{DemoKitGUI.Bullet} TMP Essential Resources");
                }
            );

            DemoKitGUI.EnabledSwitcher(
                !isTmpImported,
                _ =>
                {
                    DemoKitGUI.IndentBlock(() =>
                    {
                        DemoKitGUI.Label($"<b>{DemoKitUtils.PackageName}</b> uses TMP Essential Resources in the demo main scene. Please import TMP Essential Resources via bellow button:");

                        DemoKitGUI.SmallSpace();

                        DemoKitGUI.Button(
                            "Import TMP Essential Resources",
                            () =>
                            {
                                needsRepaint |= EditorApplication.ExecuteMenuItem("Window/TextMeshPro/Import TMP Essential Resources");
                            },
                            !isTmpImported
                        );
                    });
                }
            );

            if (isTmpImported)
            {
                DemoKitGUI.IndentBlock(
                    () => DemoKitGUI.AlignRight(
                        () => DemoKitGUI.Label($"* Already imported")
                    )
                );
            }

            return needsRepaint;
        }

        internal override bool ContentLayout()
        {
            var needsRepaint = false;

            needsRepaint |= TmpImportLayout();

            return needsRepaint;
        }
    }
}
