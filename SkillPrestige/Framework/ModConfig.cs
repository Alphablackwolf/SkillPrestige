using System;
using SkillPrestige.Logging;

namespace SkillPrestige.Framework
{
    /// <summary>Represents options for this mod.</summary>
    [Serializable]
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The logging verbosity for the mod. A log level set to Verbose will log all entries.</summary>
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;

        /// <summary>Whether testing mode is enabled, which adds testing specific commands to the system.</summary>
        public bool TestingMode { get; set; }
    }
}
