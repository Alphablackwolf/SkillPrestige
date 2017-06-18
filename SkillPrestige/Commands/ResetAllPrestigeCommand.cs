using System;
using System.Collections.Generic;
using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige.Commands
{
    /// <summary>A command that resets the player's professions after all professions has been removed.</summary>
    internal class ResetAllPrestigeCommand : SkillPrestigeCommand
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public ResetAllPrestigeCommand()
            : base("player_resetallprestige", "Resets all prestige professions and prestige points.\n\nUsage: player_resetallprestige", isTestCommand: true) { }

        /// <summary>Invokes the command when called from the console.</summary>
        /// <param name="args">The command-line arguments.</param>
        public override void Apply(string[] args)
        {
            if (Game1.player == null)
            {
                SkillPrestigeMod.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }
            SkillPrestigeMod.LogMonitor.Log("This command will reset your character's prestiged selections and prestige points. " + Environment.NewLine +
                       "it is recommended you run the player_resetAllProfessions command after running this command." + Environment.NewLine +
                       "Please note that this command by itself will only clear the prestige data located in the skills prestige mod folder, " +
                       "and *not* the player's gained professions. once this is run all professions already prestiged/purchased will still belong to the player." + Environment.NewLine +
                       "If you have read this and wish to continue confirm with 'y' or 'yes'");
            var response = Console.ReadLine();
            if (response == null || (!response.Equals("y", StringComparison.InvariantCultureIgnoreCase) && !response.Equals("yes", StringComparison.InvariantCultureIgnoreCase)))
            {
                Logger.LogVerbose("Cancelled all prestige reset.");
                return;
            }
            Logger.LogInformation("Resetting all skill prestiges...");
            foreach (var prestige in PrestigeSaveData.CurrentlyLoadedPrestigeSet.Prestiges)
            {
                prestige.PrestigePoints = 0;
                prestige.PrestigeProfessionsSelected = new List<int>();
            }
            PrestigeSaveData.Instance.Save();
            Logger.LogInformation("Prestiges reset.");
        }
    }
}
