using System.Linq;
using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige.Commands
{
    /// <summary>A command that resets the player's professions after all professions has been removed.</summary>
    internal class GetAllProfessionsCommand : SkillPrestigeCommand
    {
        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        public GetAllProfessionsCommand()
            : base("player_getallprofessions", "Returns a list of all professions the player has.\n\nUsage: player_getallprofessions", isTestCommand: true) { }

        /// <summary>Invokes the command when called from the console.</summary>
        /// <param name="args">The command-line arguments.</param>
        public override void Apply(string[] args)
        {
            const string professionSeparator = ", ";
            if (Game1.player == null)
            {
                SkillPrestigeMod.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }
            Logger.LogInformation("getting list of all professions...");
            foreach (var skill in Skill.AllSkills)
            {
                var allObtainedProfessions = skill.Professions.Where(x => Game1.player.professions.Contains(x.Id));
                var professionNames = string.Join(professionSeparator, allObtainedProfessions.Select(x => x.DisplayName).ToArray()).TrimEnd(professionSeparator.ToCharArray());
                SkillPrestigeMod.LogMonitor.Log($"{skill.Type.Name} skill (level: {skill.GetSkillLevel()}) professions: {professionNames}");
            }
            Logger.LogInformation("list of all professions retrieved.");
        }
    }
}
