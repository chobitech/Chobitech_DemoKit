// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] A unified GUI toolkit for the DemoKit Editor interface.
    /// It provides standardized styles, custom layouts (indent blocks, scrolling containers), 
    /// and reusable UI components like link labels, check-marks with status icons, 
    /// and change-tracking wrappers to ensure a consistent and interactive user experience.
    /// </summary>
    internal static class DemoKitGUI
    {
        internal static readonly GUIStyle LargeHeadingStyle = new(EditorStyles.label)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            richText = true
        };

        internal static readonly GUIStyle MediumHeadingStyle = new(EditorStyles.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            richText = true
        };

        internal static readonly GUIStyle SmallHeadingStyle = new(EditorStyles.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            wordWrap = true,
            richText = true,
        };

        internal static readonly GUIStyle RichTextStyle = new(EditorStyles.label)
        {
            richText = true,
            wordWrap = true
        };

        internal static readonly Color SeparatorColor = new(1f, 1f, 1f, 0.15f);

        private static readonly Dictionary<int, GUIStyle> _paddingStyleMap = new();

        
        internal static void SmallSpace() => EditorGUILayout.Space(6);
        internal static void MediumSpace() => EditorGUILayout.Space(10);
        internal static void LargeSpace() => EditorGUILayout.Space(16);

        internal static void LargeHeading(string text) => EditorGUILayout.LabelField(text, LargeHeadingStyle);
        internal static void MediumHeading(string text) => EditorGUILayout.LabelField(text, MediumHeadingStyle);
        internal static void SmallHeading(string text) => EditorGUILayout.LabelField(text, SmallHeadingStyle);

        internal const string Bullet = "\u2022";

        internal static void Label(string text, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(text, style: RichTextStyle, options);
        }

        private static readonly Lazy<GUIStyle> lazyLinkStyle = new(() =>
        {
            var style = new GUIStyle(EditorStyles.label);
            style.normal.textColor = new Color(0.3f, 0.5f, 1f);
            style.hover.textColor = Color.cyan;
            style.fontStyle = FontStyle.Normal;
            style.wordWrap = true;
            return style;
        });

        internal static void LinkLabel(string url, string label = null)
        {
            label ??=  url;

            var rect = GUILayoutUtility.GetRect(new GUIContent(label), lazyLinkStyle.Value, GUILayout.ExpandWidth(false));

            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);

            if (GUI.Button(rect, label, lazyLinkStyle.Value))
            {
                Application.OpenURL(url);
            }
        }

        internal static void EnabledSwitcher(bool enabled, UnityAction<bool> layout)
        {
            bool curState = GUI.enabled;

            if (!enabled || !curState)
            {
                try
                {
                    GUI.enabled = false;
                    layout?.Invoke(false);
                }
                finally
                {
                    GUI.enabled = curState;
                }
            }
            else
            {
                layout?.Invoke(true);
            }
        }

        internal static void WhenTMPImported(UnityAction<bool> layout)
        {
            var imported = TMPImportedChecker.IsImported;
            EnabledSwitcher(imported, layout);
        }

        internal static void InBulletPoints(params string[] items)
        {
            foreach (var i in items)
            {
                Label($"{Bullet} {i}");
            }
        }

        internal static void Button(string label, UnityAction onButtonClicked, bool enabled)
        {
            EnabledSwitcher(enabled, b =>
            {
                 if (GUILayout.Button(label))
                {
                    onButtonClicked?.Invoke();
                }
            });
        }

        internal static void Separator(float alpha = 1f)
        {
            EditorGUILayout.Space(4);

            var rect = EditorGUILayout.GetControlRect(false, 1);
            var color = SeparatorColor;
            color.a *= alpha;
            EditorGUI.DrawRect(rect, color);

            EditorGUILayout.Space(4);
        }

        internal static void SeparatorAndSpace(float alpha = 0.5f)
        {
            Separator(alpha);
            SmallSpace();
        }


        internal static void IndentBlock(UnityAction layoutAction, int indentSize = 16)
        {
            if (!_paddingStyleMap.TryGetValue(indentSize, out var style))
            {
                style = new GUIStyle()
                {
                    padding = new RectOffset(indentSize, 0, 0, 0)
                };
                _paddingStyleMap[indentSize] = style;
            }

            EditorGUILayout.BeginVertical(style);

            layoutAction?.Invoke();

            EditorGUILayout.EndVertical();
        }

        internal static void InHorizontalLayout(UnityAction<EditorGUILayout.HorizontalScope> layoutAction)
        {
            using var scope = new EditorGUILayout.HorizontalScope();
            layoutAction?.Invoke(scope);
        }

        internal static bool WithChangeCheck(UnityAction layoutAction, UnityAction onChanged)
        {
            EditorGUI.BeginChangeCheck();

            layoutAction?.Invoke();

            var isChanged = EditorGUI.EndChangeCheck();

            if (isChanged)
            {
                onChanged?.Invoke();
            }

            return isChanged;
        }


        internal static T WithObjectChangeCheck<T>(string title, T obj, UnityAction<T> onChanged) where T : UnityEngine.Object
        {
            var checkObj = obj;

            var changed = WithChangeCheck(
                () =>
                {
                    checkObj = (T)EditorGUILayout.ObjectField(
                        title,
                        checkObj,
                        typeof(T),
                        false
                    );
                },
                () => onChanged?.Invoke(checkObj)
            );

            return changed ? checkObj : obj;
        }


        internal static bool TextField(UnityAction<string> onTextChanged, string text = "")
        {
            string input = null;
            
            return WithChangeCheck(
                () =>
                {
                    input = EditorGUILayout.TextField(text);
                },
                () =>
                {
                    onTextChanged?.Invoke(input);
                }
            );
        }


        internal static void InfoBox(string text) => EditorGUILayout.HelpBox(text, MessageType.Info);
        internal static void WarningBox(string text) => EditorGUILayout.HelpBox(text, MessageType.Warning);
        internal static void ErrorBox(string text) => EditorGUILayout.HelpBox(text, MessageType.Error);

        internal class PaddingAndScroll
        {
            private static readonly RectOffset defaultPadding = new(16, 16, 10, 10);

            private Vector2 _scrollPos;

            private readonly RectOffset _padding;

            internal PaddingAndScroll(RectOffset padding = null)
            {
                _padding = padding ?? defaultPadding;
            }

            internal void Show(
                UnityAction layoutVertical
            )
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

                GUILayout.Space(_padding.top);

                InHorizontalLayout(
                    hScope =>
                    {
                        GUILayout.Space(_padding.left);
                        EditorGUILayout.BeginVertical();
                        layoutVertical?.Invoke();
                        EditorGUILayout.EndVertical();
                        GUILayout.Space(_padding.right);
                    }
                );

                GUILayout.Space(_padding.bottom);

                EditorGUILayout.EndScrollView();
            }
        }

        internal static void WithAlpha(float alpha, UnityAction layout)
        {
            var curColor = GUI.color;

            try
            {
                GUI.color = new Color(curColor.r, curColor.g, curColor.b, alpha);
                layout?.Invoke();
            }
            finally
            {
                GUI.color = curColor;
            }
        }

        internal static bool CheckBox(GUIContent label, bool isChecked, UnityAction<bool> onCheckChanged)
        {
            var needsRepaint = false;

            var chk = isChecked;

            WithChangeCheck(
                () =>
                {
                    chk = EditorGUILayout.ToggleLeft(label, chk);
                },
                () =>
                {
                    if (chk != isChecked)
                        {
                            onCheckChanged?.Invoke(chk);
                            needsRepaint |= true;
                        }
                }
            );

            return needsRepaint;
        }

        internal static bool CheckBox(string label, bool isChecked, UnityAction<bool> onCheckChanged)
            => CheckBox(new GUIContent(label), isChecked, onCheckChanged);

        internal static readonly Color _checkedColor = new(0f, 1f, 0f);
        internal static readonly Color _warningColor = new(1f, 0.85f, 0f);
        internal static readonly Color _unCheckedColor = new(1f, 1f, 1f, 0.2f);

        internal const float iconSize = 20;

        internal const string checkIconGuid = "e270df84b2854fc47ac517584e63d616";
        internal const string warningIconGuid = "4c7fc4195746b5540bfbc8fdaf9b4348";

        internal static readonly Lazy<Texture2D> _lazyCheckIcon = new(() =>
        {
            return DemoKitEditorUtils.LoadObjectFromGUID<Texture2D>(checkIconGuid);
        });

        internal static readonly Lazy<Texture2D> _lazyWaningIcon = new(() =>
        {
            return DemoKitEditorUtils.LoadObjectFromGUID<Texture2D>(warningIconGuid);
        });

        internal static void WithCheckMark(bool? isChecked, UnityAction layout)
        {
            if (_lazyCheckIcon.Value == null || _lazyWaningIcon.Value == null)
            {
                layout?.Invoke();
                return;
            }

            if (isChecked != null)
            {
                InHorizontalLayout(hScope =>
                {
                    layout?.Invoke();

                    var curColor = GUI.color;

                    try
                    {
                        var iconColor = isChecked.Value ? _checkedColor : _warningColor;
                        if (!GUI.enabled)
                        {
                            iconColor.a *= 0.5f;
                        }
                        GUI.color = iconColor;
                        var rect = GUILayoutUtility.GetRect(iconSize, iconSize);
                        GUI.DrawTexture(
                            rect,
                            isChecked.Value ? _lazyCheckIcon.Value : _lazyWaningIcon.Value,
                            ScaleMode.ScaleToFit
                        );
                    }
                    finally
                    {
                        GUI.color = curColor;
                    }


                    GUILayout.FlexibleSpace();
                });
            }
            else
            {
                layout?.Invoke();
            }
        }

        internal class Expandable
        {
            internal readonly string titleLabel;

            internal Expandable(string titleLabel)
            {
                this.titleLabel = titleLabel;
            }

            internal bool IsExpanded { get; private set; }

            internal void ShowLayout(UnityAction layout, bool expanded, UnityAction<bool> isOpenStateChanged = null)
            {
                IsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(expanded, titleLabel);

                if (IsExpanded)
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        layout?.Invoke();
                    }
                }

                isOpenStateChanged?.Invoke(IsExpanded);

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        private static readonly Dictionary<Color, GUIStyle> _bgColorStyleMap = new();

        private static GUIStyle GetBgColorStyle(Color color)
        {
            if (!_bgColorStyleMap.TryGetValue(color, out var style))
            {
                style = new GUIStyle(GUI.skin.box);
                style.normal.background = Texture2D.whiteTexture;
                _bgColorStyleMap[color] = style;
            }
            return style;
        }

        internal static bool ColoredArea(Color? color, System.Func<bool> layoutFunc)
        {
            if (color == null)
            {
                return layoutFunc();
            }
            
            
            var bgStyle = GetBgColorStyle(color.Value);

            var curColor = GUI.backgroundColor;

            GUI.backgroundColor = color.Value;

            EditorGUILayout.BeginVertical(bgStyle);

            GUI.backgroundColor = curColor;

            var needsRepaint = false;

            needsRepaint |= layoutFunc();

            EditorGUILayout.EndVertical();

            return needsRepaint;
        }

        internal static void AlignRight(UnityAction layout)
        {
            if (layout == null)
            {
                return;
            }

            InHorizontalLayout(hScope =>
            {
                GUILayout.FlexibleSpace();
                layout?.Invoke();
            });
        }
    }
}