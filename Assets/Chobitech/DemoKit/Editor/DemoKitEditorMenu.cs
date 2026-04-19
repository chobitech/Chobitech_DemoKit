// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php


using System.Linq;
using System.Text;

namespace Chobitech.DemoKit.Editor
{
    /// <summary>
    /// [Internal] Centralizes menu path definitions and navigation string utilities for the DemoKit Editor.
    /// It provides consistent naming conventions for Unity's top-level menus (Tools, Window) 
    /// and the Assets creation context menu, ensuring a unified user interface and 
    /// making it easier to generate descriptive breadcrumb navigation for documentation or UI labels.
    /// </summary>
    internal static class DemoKitEditorMenu
    {
        internal const string BaseMenuPath = "Chobitech/DemoKit";

        internal const string ToolsMenuPathPrefix = "Tools/" + BaseMenuPath;
        internal const string WindowMenuPathPrefix = "Window/" + BaseMenuPath;

        internal const string AssetCreateMenuPathPrefix = "Assets/Create/" + BaseMenuPath;

        internal const string SetupWindowName = "Demo Setup Window";

        internal const string SetupWindowPath = WindowMenuPathPrefix + "/" + SetupWindowName;

        internal const string DemoKitProjectSettingsPath = "{BaseMenuPath} Setup";

        internal static string[] GetMenuPathItemList(string menuPath)
        {
            return menuPath.Split("/");
        }

        internal static string GetMenuPathNavigationString(string pathPrefix, params string[] menuItems)
        {
            var items = GetMenuPathItemList(pathPrefix).ToList();
            items.AddRange(menuItems);
            var sb = new StringBuilder();
            sb.AppendJoin(" > ", items.Select(item => $"[{item}]"));
            return sb.ToString();
        }

        internal static string GetAssetCreateMenuPathString(params string[] menuItems)
            => GetMenuPathNavigationString(AssetCreateMenuPathPrefix, menuItems);

        internal static string GetToolsMenuPathString(params string[] menuItems)
            => GetMenuPathNavigationString(ToolsMenuPathPrefix, menuItems);
    }
}