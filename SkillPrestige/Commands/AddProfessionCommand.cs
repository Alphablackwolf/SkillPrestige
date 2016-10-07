using System;
using System.Collections.Generic;
using System.Linq;
using SkillPrestige.Logging;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SkillPrestige.Commands
{
    /// <summary>
    /// Represents the command that clears all professions from a player's game.
    /// </summary>
    // ReSharper disable once UnusedMember.Global - referenced via reflection
    internal class AddProfessionCommand : SkillPrestigeCommand
    {

        public AddProfessionCommand() : base("player_addprofession", "Adds the specified profession to the player | player_addprofession <profession>", GetArgumentDescriptions()) { }

        private static IEnumerable<string> GetArgumentDescriptions()
        {
            const string professionSeparator = ", ";
            var professionNames =
                string.Join(professionSeparator, Skill.AllSkills.SelectMany(x => x.Professions).Select(x => x.DisplayName).ToArray())
                    .TrimEnd(professionSeparator.ToCharArray());
            return new[]
            {
                $"({professionNames})<profession>"
            };
        }

        protected override bool TestingCommand => false;

        protected override void ApplyCommandEffect(object sender, EventArgsCommand e)
        {
            if (e.Command.CalledArgs.Length < 1)
            {
                Log.AsyncR("<profession> must be specified");
                return;
            }
            var professionArgument = e.Command.CalledArgs[0];
            if (!Skill.AllSkills.SelectMany(x => x.Professions).Select(x => x.DisplayName).Contains(professionArgument, StringComparer.InvariantCultureIgnoreCase))
            {
                Log.AsyncR("<profession> is invalid");
                return;
            }
            if (Game1.player == null)
            {
                Log.AsyncR("A game file must be loaded in order to run this command.");
                return;
            }
            var profession = Skill.AllSkills.SelectMany(x => x.Professions).Single(x => x.DisplayName.Equals(professionArgument, StringComparison.InvariantCultureIgnoreCase));
            if (Game1.player.professions.Contains(profession.Id))
            {
                Log.AsyncR("profession already added.");
            }
            Logger.LogInformation($"Adding profession {professionArgument}...");
            Game1.player.professions.Add(profession.Id);
            profession.SpecialHandling?.ApplyEffect();
            Logger.LogInformation($"Profession {professionArgument} added.");
        }
    }
}
