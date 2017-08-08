using System;
using System.Collections.Generic;
using System.Linq;

namespace SkillPrestige
{
    /// <summary>
    /// Math Extension methods created and used for SkillPrestige.
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// Checks if any of the objects passed equal the referenced object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="itemsToCheck"></param>
        /// <returns></returns>
        public static bool IsOneOf<T>(this T item, params T[] itemsToCheck)
        {
            return itemsToCheck.Contains(item);
        }

        /// <summary>
        /// Checks if any of the objects passed equal the referenced object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="itemsToCheck"></param>
        /// <returns></returns>
        public static bool IsOneOf<T>(this T item, IEnumerable<T> itemsToCheck)
        {
            return itemsToCheck.Contains(item);
        }

        /// <summary>
        /// Checks to see if the text starts with any one of the passed strings.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="stringsToCheck"></param>
        /// <returns></returns>
        // ReSharper disable once MemberCanBePrivate.Global - I want this to be available.
        public static bool StartsWithOneOf(this string text, params string[] stringsToCheck)
        {
            return stringsToCheck.Any(text.StartsWith);
        }

        /// <summary>
        /// Clamps a value within a given range.
        /// </summary>
        /// <typeparam name="T">Type of item to compare. Must be IComparable.</typeparam>
        /// <param name="value">Value to clamp.</param>
        /// <param name="minimum">Minimum allowable value.</param>
        /// <param name="maximum">Maximum allowable value.</param>
        /// <returns>The value given as long as it is in the given range, otherwise it gives the closest available value within the range (inclusive).</returns>
        public static T Clamp<T>(this T value, T minimum, T maximum) where T : IComparable<T>
        {
            if(minimum.CompareTo(maximum) > 0) throw new ArgumentException("Minimum cannot exceed maximum.");
            return value.CompareTo(minimum) < 0 ? minimum : value.CompareTo(maximum) > 0 ? maximum : value;
        }

        /// <summary>
        /// Detects if a value is in a given set.
        /// </summary>
        /// <typeparam name="T">The type of item to check.</typeparam>
        /// <param name="value">the value to look for in the given array.</param>
        /// <param name="items">The parameter array of values to compare to the given value.</param>
        /// <returns>True if the value given is in the array of comparison items.</returns>
        public static bool In<T>(this T value, params T[] items)
        {
            return items.Contains(value);
        }

        /// <summary>
        ///  Detects if a value is in a given set.
        /// </summary>
        /// <typeparam name="T">The type of item to check.</typeparam>
        /// <param name="value">the value to look for in the given enumeration.</param>
        /// <param name="items">The enumeration of values to compare to the given value.</param>
        /// <returns>True if the value given is in the enumeration of comparison items.</returns>
        public static bool In<T>(this T value, IEnumerable<T> items)
        {
            return items.Contains(value);
        }

        /// <summary>
        /// Returns Math.Floor as an integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Floor(this decimal value)
        {
            return Convert.ToInt32(Math.Floor(value));
        }

        /// <summary>
        /// Returns Math.Floor as an integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Floor(this float value)
        {
            return Convert.ToInt32(Math.Floor(value));
        }

        /// <summary>
        /// Returns Math.Floor as an integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Floor(this double value)
        {
            return Convert.ToInt32(Math.Floor(value));
        }

        /// <summary>
        /// Returns Math.Ceiling as an integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Ceiling(this decimal value)
        {
            return Convert.ToInt32(Math.Ceiling(value));
        }

        /// <summary>
        /// Returns Math.Ceiling as an integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Ceiling(this float value)
        {
            return Convert.ToInt32(Math.Ceiling(value));
        }

        /// <summary>
        /// Returns Math.Ceiling as an integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Ceiling(this double value)
        {
            return Convert.ToInt32(Math.Ceiling(value));
        }
    }
}
