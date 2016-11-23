using System;
using System.Collections.Generic;
using System.Linq;
using SkillPrestige.Logging;
using StardewModdingAPI.Events;
using StardewValley;

namespace SkillPrestige.Commands
{
    /// <summary>
    /// Represents the command that sets experience levels for a player.
    /// </summary>
    /// // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class SetExperienceCommand : SkillPrestigeCommand
    {

        public SetExperienceCommand() : base("player_setexperience", "Sets the player's specified skill to the specified level of experience | player_setexperience <skill> <value>", GetArgumentDescriptions()) { }

        private static IEnumerable<string> GetArgumentDescriptions()
        {
            const string skillSeparator = ", ";
            var skillNames =
                string.Join(skillSeparator, Skill.AllSkills.Select(x => x.Type.Name).ToArray())
                    .TrimEnd(skillSeparator.ToCharArray());
            return new[]
            {
                $"({skillNames})<skill> (0-15000)<value> The target experience level"
            };
        }

        protected override bool TestingCommand => true;

        protected override void ApplyCommandEffect(object sender, EventArgsCommand e)
        {
            if (e.Command.CalledArgs.Length <= 1)
            {
                SkillPrestigeMod.LogMonitor.Log("<skill> and <value> must be specified");
                return;
            }
            var skillArgument = e.Command.CalledArgs[0];
            if (!Skill.AllSkills.Select(x => x.Type.Name).Contains(skillArgument, StringComparer.InvariantCultureIgnoreCase))
            {
                SkillPrestigeMod.LogMonitor.Log("<skill> is invalid");
                return;
            }
            int experienceArgument;
            if (!int.TryParse(e.Command.CalledArgs[1], out experienceArgument))
            {
                SkillPrestigeMod.LogMonitor.Log("experience must be an integer.");
                return;
            }
            if (Game1.player == null)
            {
                SkillPrestigeMod.LogMonitor.Log("A game file must be loaded in order to run this command.");
                return;
            }
            Logger.LogInformation("Setting experience level...");
            Logger.LogVerbose($"experience argument: {experienceArgument}");
            experienceArgument = experienceArgument.Clamp(0, 15000);
            Logger.LogVerbose($"experience used: {experienceArgument}");
            var skill = Skill.AllSkills.Single(x => x.Type.Name.Equals(skillArgument, StringComparison.InvariantCultureIgnoreCase));
            var playerSkillExperience = Game1.player.experiencePoints[skill.Type.Ordinal];
            Logger.LogVerbose($"Current experience level for {skill.Type.Name} skill: {playerSkillExperience}");
            Logger.LogVerbose($"Setting {skill.Type.Name} skill to {experienceArgument} experience.");
            skill.SetSkillExperience(experienceArgument);
            var skillLevel = GetLevel(experienceArgument);
            Logger.LogVerbose($"Setting skill level for {experienceArgument} experience: {skillLevel}");
            skill.SetSkillLevel(skillLevel);
        }

        private static int GetLevel(int experience)
        {
            if (experience < 100) return 0;
            if (experience < 380) return 1;
            if (experience < 770) return 2;
            if (experience < 1300) return 3;
            if (experience < 2150) return 4;
            if (experience < 3300) return 5;
            if (experience < 4800) return 6;
            if (experience < 6900) return 7;
            if (experience < 10000) return 8;
            return experience < 15000 ? 9 : 10;
        }
    }
}
