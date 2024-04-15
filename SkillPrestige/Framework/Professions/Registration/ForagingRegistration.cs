using System.Collections.Generic;
using SkillPrestige.Logging;
using SkillPrestige.Professions;

namespace SkillPrestige.Framework.Professions.Registration
{
    // ReSharper disable once UnusedMember.Global - created through reflection.
    internal sealed class ForagingRegistration : ProfessionRegistration
    {
        /*********
        ** Public methods
        *********/
        public override void RegisterProfessions()
        {
            Logger.LogInformation("Registering Foraging professions...");
            Forester = new TierOneProfession
            {
                Id = 12
            };
            Gatherer = new TierOneProfession
            {
                Id = 13
            };
            Lumberjack = new TierTwoProfession
            {
                Id = 14,
                TierOneProfession = Forester
            };
            Tapper = new TierTwoProfession
            {
                Id = 15,
                TierOneProfession = Forester
            };
            Botanist = new TierTwoProfession
            {
                Id = 16,
                TierOneProfession = Gatherer
            };
            Tracker = new TierTwoProfession
            {
                Id = 17,
                TierOneProfession = Gatherer
            };
            Forester.TierTwoProfessions = new List<TierTwoProfession>
            {
                Lumberjack,
                Tapper
            };
            Gatherer.TierTwoProfessions = new List<TierTwoProfession>
            {
                Botanist,
                Tracker
            };
            Logger.LogInformation("Foraging professions registered.");
        }
    }
}
