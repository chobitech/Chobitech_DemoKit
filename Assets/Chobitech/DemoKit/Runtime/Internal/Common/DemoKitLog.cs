// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using System;
using UnityEngine;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// Provides a standardized logging interface for the DemoKit package.
    /// All logs are prefixed with a rich text tag and are conditionally compiled to be editor-only.
    /// </summary>
    public static class DemoKitLog
    {
        private const string Tag = "<b>[DemoKit]</b>";
        private static string WithTag(object obj) => $"{Tag} {obj}";

        /// <summary>
        /// Logs a standard information message to the Unity Console.
        /// This method is only active in the Unity Editor.
        /// </summary>
        /// <param name="obj">The message or object to log.</param>
        /// <param name="context">Object to which the message applies (allows clicking the log to highlight the object).</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Info(object obj, UnityEngine.Object context = null)
        {
            Debug.Log(WithTag(obj), context);
        }

        /// <summary>
        /// Logs a warning message to the Unity Console.
        /// This method is only active in the Unity Editor.
        /// </summary>
        /// <param name="obj">The message or object to log.</param>
        /// <param name="context">Object to which the message applies.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Warning(object obj, UnityEngine.Object context = null)
        {
            Debug.LogWarning(WithTag(obj), context);
        }

        /// <summary>
        /// Logs an error message to the Unity Console.
        /// This method is only active in the Unity Editor.
        /// </summary>
        /// <param name="obj">The message or object to log.</param>
        /// <param name="context">Object to which the message applies.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Error(object obj, UnityEngine.Object context = null)
        {
            Debug.LogError(WithTag(obj), context);
        }

        /// <summary>
        /// Logs an exception to the Unity Console with a tagged message.
        /// This method is only active in the Unity Editor.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        /// <param name="context">Object to which the exception applies.</param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Exception(Exception ex, UnityEngine.Object context = null)
        {
            var taggedEx = new Exception(WithTag(ex.Message), ex);
            Debug.LogException(taggedEx, context);
        }
    }
}