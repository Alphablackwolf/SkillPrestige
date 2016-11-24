using System;
using System.Linq;
using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige
{
    /// <summary>
    /// Handles experience adjustments for skills.
    /// </summary>
    public static class ExperienceHandler
    {
        private static bool ExperienceLoaded { get; set; }

        private static int[] LastExperiencePoints { get; set; }

        public static void UpdateExperience()
        {
            if (!PerSaveOptions.Instance.UseExperienceMultiplier) return;
            if (!ExperienceLoaded)
            {
                ExperienceLoaded = true;
                LastExperiencePoints = (int[])Game1.player.experiencePoints.Clone();
                Logger.LogVerbose("Loaded Experience state.");
                return;
                
            }
            for(var skillIndex = 0; skillIndex < Game1.player.experiencePoints.Length; skillIndex++)
            {
                var lastExperienceDetected = LastExperiencePoints[skillIndex];
                var currentExperience = Game1.player.experiencePoints[skillIndex];
                var gainedExperience = currentExperience - lastExperienceDetected;
                var skillExperienceFactor = PrestigeSaveData.CurrentlyLoadedPrestigeSet.Prestiges.Single(x => x.SkillType.Ordinal == skillIndex).PrestigePoints * PerSaveOptions.Instance.ExperienceMultiplier;
                if (gainedExperience <= 0 || skillExperienceFactor <= 0) continue;
                Logger.LogVerbose($"Detected {gainedExperience} experience gained in {Skill.AllSkills.Single(x => x.Type.Ordinal == skillIndex).Type.Name} skill.");
                var extraExperience = (int)Math.Floor(gainedExperience*skillExperienceFactor);
                Logger.LogVerbose($"Adding {extraExperience} experience to {Skill.AllSkills.Single(x => x.Type.Ordinal == skillIndex).Type.Name} skill.");
                Game1.player.gainExperience(skillIndex, extraExperience);
            }
            LastExperiencePoints = (int[])Game1.player.experiencePoints.Clone();
        }
    }
}
