using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M7.Extensions.Text {
    public static class TextExtensions {

        public static string Bold(this string value) {
            return string.Format("<b>{0}</b>", value);
        }

        public static string Italic(this string value) {
            return string.Format("<i>{0}</i>", value);
        }

        public static string Underline(this string value) {
            return string.Format("<u>{0}</u>", value);
        }

        public static string Font(this string value, string font) {
            return string.Format("<font=\"{1}\">{0}</font>", value, font);
        }

        public static string Color(this string value, Color color) {
            var hex = ColorUtility.ToHtmlStringRGBA(color);
            return string.Format("<color=#{0}>{1}</color>", hex, value);
        }

        public static string Size(this string value, string size) {
            return $"<size={size}>{value}</size>";
        }

        public static string ToUppercaseFirst(this string value, bool lowerCaseOther = true)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(value[0]) + (lowerCaseOther ? value.Substring(1).ToLower() : value.Substring(1));
        }
    }
}