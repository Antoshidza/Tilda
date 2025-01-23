using UnityEngine;

namespace Tilda
{
    internal static class ConsoleUtils
    {
        public static string Colored(this string message, in Color color)
            => $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
    }
}