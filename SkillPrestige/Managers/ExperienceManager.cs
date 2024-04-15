﻿using System.Linq;
using SkillPrestige.Extensions;
using SkillPrestige.Logging;
using SkillPrestige.Options;
using SkillPrestige.PrestigeFramework;
using SkillPrestige.Skills;
using StardewValley;

namespace SkillPrestige.Managers
{
    /// <summary>
    /// Handles experience adjustments for skills.
    /// </summary>
    public static class ExperienceManager
    {
        private static bool _disableExperienceGains;

        private static bool ExperienceLoaded { get; set; }

        private static int[] LastExperiencePoints { get; set; }

        public static bool DisableExperienceGains
        {
            private get => _disableExperienceGains;
            set
            {
                if (_disableExperienceGains != value) Logger.LogInformation($"{(value ? "Enabling" : "Disabling")} experience gains from prestige points...");
                _disableExperienceGains = value;
                ResetExperience();
            }
        }

        public static void ResetExperience()
        {
            ExperienceLoaded = false;
            LastExperiencePoints = null;
        }

        public static void UpdateExperience()
        {
            if (DisableExperienceGains || !PerSaveOptions.Instance.UseExperienceMultiplier) return;
            if (!ExperienceLoaded)
            {
                ExperienceLoaded = true;
                LastExperiencePoints = (int[])Game1.player.experiencePoints.Clone();
                Logger.LogVerbose("Loaded Experience state.");
                return;
            }
            if (Game1.player.experiencePoints.SequenceEqual(LastExperiencePoints)) return;
            if (Game1.player.experiencePoints.Length != LastExperiencePoints.Length)
            {
                LastExperiencePoints = (int[])Game1.player.experiencePoints.Clone();
                return;
            }
            for (var skillIndex = 0; skillIndex < Game1.player.experiencePoints.Length; skillIndex++)
            {
                var skillHasAPrestige = Skill.AllSkills.Any(x => x.Type.Ordinal == skillIndex);
                if (!skillHasAPrestige) continue; 
                var lastExperienceDetected = LastExperiencePoints[skillIndex];
                var currentExperience = Game1.player.experiencePoints[skillIndex];
                var gainedExperience = currentExperience - lastExperienceDetected;
                var skillExperienceFactor = PrestigeSaveData.CurrentlyLoadedPrestigeSet.Prestiges.Single(x => x.SkillType.Ordinal == skillIndex).PrestigePoints * PerSaveOptions.Instance.ExperienceMultiplier;
                if (gainedExperience <= 0 || skillExperienceFactor <= 0) continue;
                Logger.LogVerbose($"Detected {gainedExperience} experience gained in {Skill.AllSkills.Single(x => x.Type.Ordinal == skillIndex).Type.Name} skill.");
                var extraExperience = (gainedExperience * skillExperienceFactor).Floor();
                Logger.LogVerbose($"Adding {extraExperience} experience to {Skill.AllSkills.Single(x => x.Type.Ordinal == skillIndex).Type.Name} skill.");
                Game1.player.gainExperience(skillIndex, extraExperience);
            }
            LastExperiencePoints = (int[])Game1.player.experiencePoints.Clone();
        }
    }
}