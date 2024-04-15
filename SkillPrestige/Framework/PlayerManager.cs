using SkillPrestige.Logging;
using SkillPrestige.SkillTypes;
using StardewValley;

namespace SkillPrestige.Framework
{
    /// <summary>A class to manage aspects of the player.</summary>
    internal static class PlayerManager
    {
        /*********
        ** Fields
        *********/
        private static int OriginalMaxHealth = 100;


        /*********
        ** Public methods
        *********/
        public static void CorrectStats(Skill skillThatIsReset)
        {
            if (skillThatIsReset.Type != SkillType.Combat)
                Logger.LogVerbose("Player Manager - no stats reset.");
            else
            {
                Logger.LogVerbose($"Player Manager- Combat reset. Resetting max health to {OriginalMaxHealth}.");
                Game1.player.maxHealth = OriginalMaxHealth;
            }
        }
    }
}
