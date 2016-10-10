using System;
using System.IO;
using JsonNet.PrivateSettersContractResolvers;
using Newtonsoft.Json;
using SkillPrestige.Logging;

namespace SkillPrestige
{
    /// <summary>
    /// Represents options for this mod per save file.
    /// </summary>
    [Serializable]
    public class PerSaveOptions
    {
        /// <summary>
        /// Whether or not to reset the recipes of a skill on load.
        /// </summary>
        public bool ResetRecipesOnPrestige { get; set; }

        private PerSaveOptions() { }
        private static PerSaveOptions _instance;
        public static PerSaveOptions Instance 
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new PerSaveOptions();
                LoadPerSaveOptions();
                return _instance;
            }
        }

       public static void LoadPerSaveOptions()
        {
            Logger.LogInformation($"per save options file path: {SkillPrestigeMod.CurrentSaveOptionsPath}");
            if (!File.Exists(SkillPrestigeMod.CurrentSaveOptionsPath)) SetupPerSaveOptionsFile();
            var settings = new JsonSerializerSettings { ContractResolver = new PrivateSetterContractResolver() };
            Logger.LogInformation("Deserializing per save options file...");
            _instance = JsonConvert.DeserializeObject<PerSaveOptions>(File.ReadAllText(SkillPrestigeMod.CurrentSaveOptionsPath), settings);
            Logger.LogInformation("Per save options loaded.");
        }

        private static void SetupPerSaveOptionsFile()
        {
            Logger.LogInformation("Creating new options file...");
            try
            {
                Instance.ResetRecipesOnPrestige =  true;
                Save();
            }
            catch(Exception exception)
            {
                Logger.LogError($"Error while attempting to create a per save options file. {Environment.NewLine} {exception}");
                throw;
            }
            Logger.LogInformation("Successfully created new per save options file.");
        }

        private static void Save()
        {
            File.WriteAllLines(SkillPrestigeMod.CurrentSaveOptionsPath, new[] { JsonConvert.SerializeObject(_instance) });
            Logger.LogInformation("Per save options file saved.");
        }
    }
}