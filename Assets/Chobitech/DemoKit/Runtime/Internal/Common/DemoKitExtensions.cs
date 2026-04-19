// Copyright (c) 2026 chobitech
// Released under the MIT license
// https://opensource.org/licenses/mit-license.php

using UnityEngine;
using UnityEngine.Events;

namespace Chobitech.DemoKit
{
    /// <summary>
    /// Provides a set of static extension methods used throughout the DemoKit system 
    /// for color conversion, safe Unity Object execution, and functional programming patterns.
    /// </summary>
    public static class DemoKitExtensions
    {
        /// <summary>
        /// Converts a normalized float value (0.0 to 1.0) to a byte-range integer (0 to 255).
        /// </summary>
        /// <param name="f">The normalized float value.</param>
        /// <returns>An integer clamped between 0 and 255.</returns>
        public static int ToHexInt(this float f) => (int)(Mathf.Clamp01(f) * 255);

        /// <summary>
        /// Converts an integer to a lowercase two-digit hexadecimal string.
        /// </summary>
        /// <param name="i">The integer value to convert.</param>
        /// <returns>A hexadecimal string (e.g., "ff").</returns>
        public static string ToHex(this int i) => i.ToString("x2");

        /// <summary>
        /// Converts a normalized float value to a two-digit hexadecimal string.
        /// </summary>
        /// <param name="f">The normalized float value.</param>
        /// <returns>A hexadecimal string representation of the float.</returns>
        public static string ToHex(this float f) => f.ToHexInt().ToHex();

        /// <summary>
        /// Converts a Unity Color to a hexadecimal color string with a leading hash.
        /// </summary>
        /// <param name="c">The color to convert.</param>
        /// <returns>A string in the format #RRGGBBAA.</returns>
        public static string ToHexString(this Color c)
        {
            return $"#{c.r.ToHex()}{c.g.ToHex()}{c.b.ToHex()}{c.a.ToHex()}";
        }

        /// <summary>
        /// Safely executes a function that takes a Unity Object, provided the object is not null.
        /// Logs a warning using DemoKitLog if the Unity Object is null.
        /// </summary>
        /// <typeparam name="T">The return type of the function.</typeparam>
        /// <typeparam name="U">The type of the Unity Object (must inherit from UnityEngine.Object).</typeparam>
        /// <param name="obj">The calling instance (ignored, used for extension scope).</param>
        /// <param name="unityObj">The Unity Object to validate.</param>
        /// <param name="func">The function to execute if the object is valid.</param>
        /// <param name="defVal">The default value to return if the object is null or destroyed.</param>
        /// <returns>The result of the function if unityObj is valid; otherwise, defVal.</returns>
        public static T RunIfUnityObjectIsNotNull<T, U>(this object obj, U unityObj, System.Func<U, T> func, T defVal = default) where U : Object
        {
            if (unityObj != null && func != null)
            {
                return func(unityObj);
            }

            DemoKitLog.Warning($"The instance of {typeof(U).Name} is null");
            return defVal;
        }

        /// <summary>
        /// Safely executes an action that takes a Unity Object, provided the object is not null.
        /// Logs a warning using DemoKitLog if the Unity Object is null.
        /// </summary>
        /// <typeparam name="U">The type of the Unity Object (must inherit from UnityEngine.Object).</typeparam>
        /// <param name="obj">The calling instance (ignored, used for extension scope).</param>
        /// <param name="unityObj">The Unity Object to validate.</param>
        /// <param name="action">The action to execute if the object is valid.</param>
        public static void RunIfUnityObjectIsNotNull<U>(this object obj, U unityObj, UnityAction<U> action) where U : Object
        {
            _ = obj.RunIfUnityObjectIsNotNull<bool, U>(unityObj, uo =>
            {
                action?.Invoke(uo);
                return false;
            });
        }

        /// <summary>
        /// Calls a specified function on the object and returns the result. 
        /// Useful for transforming values or scoping operations.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <typeparam name="R">The type of the result.</typeparam>
        /// <param name="t">The object instance.</param>
        /// <param name="func">The function to apply to the object.</param>
        /// <returns>The result of func, or default(R) if func is null.</returns>
        public static R Let<T, R>(this T t, System.Func<T, R> func)
        {
            if (func != null)
            {
                return func(t);
            }
            return default;
        }

        /// <summary>
        /// Calls a specified action on the object and returns the object itself. 
        /// Useful for performing side effects or configuring an object in a chain.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="t">The object instance.</param>
        /// <param name="action">The action to perform on the object.</param>
        /// <returns>The original object instance.</returns>
        public static T Also<T>(this T t, UnityAction<T> action)
            => t.Let<T, T>(st =>
            {
                action?.Invoke(st);
                return st;
            });
    }
}