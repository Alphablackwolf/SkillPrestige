using System;
using StardewModdingAPI;

namespace SkillPrestige.Logging
{
    /// <summary>
    /// A wrapper for the Stardew Valley logger to simplify the interface and restrict what is logged.
    /// </summary>
    public static class Logger
    {
        public static void LogVerbose(string message)
        {
            if(Options.Instance.LogLevel >= LogLevel.Verbose) Log.Async(FormatMessage(message));
        }

        public static void LogInformation(string message)
        {
            if (Options.Instance.LogLevel >= LogLevel.Information) Log.AsyncColour(FormatMessage(message), ConsoleColor.White);
        }

        public static void LogWarning(string message)
        {
            if (Options.Instance.LogLevel >= LogLevel.Warning) Log.AsyncY(FormatMessage(message));
        }

        public static void LogError(string message)
        {
            if (Options.Instance.LogLevel >= LogLevel.Error) Log.AsyncO(FormatMessage(message).AddErrorText());
        }

        public static void LogCritical(string message)
        {
            if (Options.Instance.LogLevel >= LogLevel.Critical) Log.AsyncR(FormatMessage(message).AddErrorText());
        }

        public static void LogCriticalWarning(string message)
        {
            if (Options.Instance.LogLevel >= LogLevel.Critical) Log.AsyncR(FormatMessage(message));
        }

        public static void LogDisplay(string message)
        {
            Log.AsyncG(message);
        }

        public static void LogPriorToOptionsLoaded(string message)
        {
            Log.AsyncColour(FormatMessage(message), ConsoleColor.White);
        }

        public static void LogOptionsError(string message)
        {
            Log.AsyncR(FormatMessage(message).AddErrorText());
        }

        private static string FormatMessage(string message)
        {
            return $"{SkillPrestigeMod.Name}: {message}";
        }

        private static string AddErrorText(this string message)
        {
            return $"{message} {Environment.NewLine} Please file a bug report on NexusMods.";
        }
    }
}
