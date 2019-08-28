using System;
using System.IO;
using Newtonsoft.Json;
using SkillPrestige.Framework.JsonNet.PrivateSettersContractResolvers;
using SkillPrestige.Logging;

namespace SkillPrestige.Framework
{
    /// <summary>Represents options for this mod.</summary>
    [Serializable]
    internal class ModConfig
    {
        /// <summary>The logging verbosity for the mod. A log level set to Verbose will log all entries.</summary>
        public LogLevel LogLevel { get; private set; }

        /// <summary>Whether testing mode is enabled, which adds testing specific commands to the system.</summary>
        public bool TestingMode { get; set; }

        private ModConfig() { }
        private static ModConfig _instance;
        public static ModConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ModConfig();
                    LoadOptions();
                }
                return _instance;
            }
        }

        private static void LoadOptions()
        {
            Logger.LogDisplay($"options file path: {ModEntry.ConfigPath}");
            if (!File.Exists(ModEntry.ConfigPath))
                SetupOptionsFile();
            var settings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };
            Logger.LogDisplay("Deserializing options file...");
            _instance = JsonConvert.DeserializeObject<ModConfig>(File.ReadAllText(ModEntry.ConfigPath), settings);
            Logger.LogInformation("Options loaded.");
        }

        private static void SetupOptionsFile()
        {
            Logger.LogDisplay("Creating new options file...");
            try
            {
                Instance.LogLevel = LogLevel.Warning;
                Save();
            }
            catch (Exception exception)
            {
                Logger.LogOptionsError($"Error while attempting to create an options file. {Environment.NewLine} {exception}");
                throw;
            }
            Logger.LogInformation("Successfully created new options file.");
        }

        private static void Save()
        {
            File.WriteAllLines(ModEntry.ConfigPath, new[] { JsonConvert.SerializeObject(_instance) });
            Logger.LogInformation("Options file saved.");
        }
    }
}
