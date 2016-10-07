using System;
using System.IO;
using JsonNet.PrivateSettersContractResolvers;
using Newtonsoft.Json;
using SkillPrestige.Logging;

namespace SkillPrestige
{
    /// <summary>
    /// Represents options for this mod.
    /// </summary>
    [Serializable]
    public class Options
    {
        /// <summary>
        /// The logging verbosity for the mod. A log level set to Verbose will log all entries.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Whether or not testing mode is enabled, which adds testing specific commands to the system.
        /// </summary>
        public bool TestingMode { get; set; }

        private Options() { }

        private const string OptionsFilePath = @"Mods\SkillPrestige\Options.json";
        private static Options _instance;
        public static Options Instance 
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new Options();
                LoadOptions();
                return _instance;
            }
            
        }

       private static void LoadOptions()
        {
            if (!File.Exists(OptionsFilePath)) SetupOptionsFile();
            var settings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };
            Logger.LogPriorToOptionsLoaded("Deserializing options file...");
            _instance = JsonConvert.DeserializeObject<Options>(File.ReadAllText(OptionsFilePath), settings);
            Logger.LogInformation("Options loaded.");
        }

        private static void SetupOptionsFile()
        {
            Logger.LogPriorToOptionsLoaded("Creating new options file...");
            try
            {
                Instance.LogLevel = LogLevel.Warning;
                Save();
            }
            catch(Exception exception)
            {
                Logger.LogOptionsError($"Error while attempting to create an options file. {Environment.NewLine} {exception}");
                throw;
            }
            Logger.LogInformation("Successfully created new options file.");
        }

        private static void Save()
        {
            File.WriteAllLines(OptionsFilePath, new[] { JsonConvert.SerializeObject(_instance) });
            Logger.LogInformation("Options file saved.");
        }

    }
}