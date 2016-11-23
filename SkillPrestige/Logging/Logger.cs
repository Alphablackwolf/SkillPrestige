using System;

namespace SkillPrestige.Logging
{
    /// <summary>
    /// A wrapper for the Stardew Valley logger to simplify the interface and restrict what is logged.
    /// </summary>
    public static class Logger
    {
        public static void LogVerbose(string message)
        {
            if(Options.Instance.LogLevel >= LogLevel.Verbose) SkillPrestigeMod.LogMonitor.Log(FormatMessage(message), StardewModdingAPI.LogLevel.Trace);
        }

        public static void LogInformation(string message)
        {
            if (Options.Instance.LogLevel >= LogLevel.Information) SkillPrestigeMod.LogMonitor.Log(FormatMessage(message), StardewModdingAPI.LogLevel.Info);
        }

        public static void LogWarning(string message)
        {
            if (Options.Instance.LogLevel >= LogLevel.Warning) SkillPrestigeMod.LogMonitor.Log(FormatMessage(message), StardewModdingAPI.LogLevel.Warn);
        }

        public static void LogError(string message)
        {
            if (Options.Instance.LogLevel >= LogLevel.Error) SkillPrestigeMod.LogMonitor.Log(FormatMessage(message).AddErrorText(), StardewModdingAPI.LogLevel.Error);
        }

        public static void LogCritical(string message)
        {
            if (Options.Instance.LogLevel >= LogLevel.Critical) SkillPrestigeMod.LogMonitor.Log(FormatMessage(message).AddErrorText(), StardewModdingAPI.LogLevel.Alert);
        }

        public static void LogCriticalWarning(string message)
        {
            if (Options.Instance.LogLevel >= LogLevel.Critical) SkillPrestigeMod.LogMonitor.Log(FormatMessage(message), StardewModdingAPI.LogLevel.Alert);
        }

        public static void LogDisplay(string message)
        {
            SkillPrestigeMod.LogMonitor.Log(FormatMessage(message));
        }

        public static void LogOptionsError(string message)
        {
            SkillPrestigeMod.LogMonitor.Log(FormatMessage(message).AddErrorText());
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
