using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SkillPrestige.LuckSkill.Framework;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using StardewModdingAPI;
using StardewValley;
using TargetMod = LuckSkill.Mod;

namespace SkillPrestige.LuckSkill
{
    /// <summary>The mod entry class.</summary>
    internal class ModEntry : Mod, ISkillMod
    {
        /*********
        ** Fields
        *********/
        /// <summary>The luck skill type.</summary>
        private SkillType SkillType;

        /// <summary>The unique ID for the Luck Skill mod.</summary>
        private readonly string TargetModId = "spacechase0.LuckSkill";


        /*********
        ** Accessors
        *********/
        /// <summary>The name to display for the mod in the log.</summary>
        public string DisplayName { get; } = "Luck Skill";

        /// <summary>Whether the mod is found in SMAPI.</summary>
        public bool IsFound { get; private set; }

        /// <summary>The skills added by this mod.</summary>
        public IEnumerable<Skill> AdditionalSkills => this.GetAddedSkills();

        /// <summary>The prestiges added by this mod.</summary>
        public IEnumerable<Prestige> AdditonalPrestiges => this.GetAddedPrestiges();


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.SkillType = new SkillType("Luck", 5);
            this.IsFound = helper.ModRegistry.IsLoaded(this.TargetModId);

            ModHandler.RegisterMod(this);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the skills added by this mod.</summary>
        private IEnumerable<Skill> GetAddedSkills()
        {
            if (!this.IsFound)
                yield break;

            yield return new Skill
            {
                Type = this.SkillType,
                SkillScreenPosition = 6,
                SourceRectangleForSkillIcon = new Rectangle(64, 0, 16, 16),
                Professions = this.GetAddedProfessions(),
                GetSkillLevel = () => Game1.player.luckLevel.Value,
                SetSkillLevel = level => Game1.player.luckLevel.Value = level
            };
        }

        /// <summary>Get the prestiges added by this mod.</summary>
        private IEnumerable<Prestige> GetAddedPrestiges()
        {
            if (!this.IsFound)
                yield break;

            yield return new Prestige
            {
                SkillType = this.SkillType
            };
        }

        /// <summary>Get the professions added by this mod.</summary>
        private IEnumerable<Profession> GetAddedProfessions()
        {
            var fortunate = new TierOneProfession
            {
                Id = TargetMod.PROFESSION_DAILY_LUCK,
                DisplayName = "Fortunate",
                EffectText = new[] { "Better daily luck." }
            };
            var popularHelper = new TierOneProfession
            {
                Id = TargetMod.PROFESSION_MORE_QUESTS,
                DisplayName = "Popular Helper",
                EffectText = new[] { "Daily quests occur three times as often." },
            };
            var lucky = new TierTwoProfession
            {
                Id = TargetMod.PROFESSION_CHANCE_MAX_LUCK,
                DisplayName = "Lucky",
                EffectText = new[] { "20% chance for max daily luck." },
                SpecialHandling = new SpecialCharmSpecialHandling(),
                TierOneProfession = fortunate
            };
            var unUnlucky = new TierTwoProfession
            {
                Id = TargetMod.PROFESSION_NO_BAD_LUCK,
                DisplayName = "Un-unlucky",
                EffectText = new[] { "Never have bad luck." },
                TierOneProfession = fortunate
            };
            var shootingStar = new TierTwoProfession
            {
                Id = TargetMod.PROFESSION_NIGHTLY_EVENTS,
                DisplayName = "Shooting Star",
                EffectText = new[] { "Nightly events occur twice as often." },
                TierOneProfession = popularHelper
            };
            var spiritChild = new TierTwoProfession
            {
                Id = TargetMod.PROFESSION_JUNIMO_HELP,
                DisplayName = "Spirit Child",
                EffectText = new[] { "Giving fits makes junimos happy. They might help your farm.\n(15% chance for some form of farm advancement.)" },
                TierOneProfession = popularHelper
            };
            fortunate.TierTwoProfessions = new List<TierTwoProfession>
            {
                lucky,
                unUnlucky
            };
            popularHelper.TierTwoProfessions = new List<TierTwoProfession>
            {
                shootingStar,
                spiritChild
            };

            return new Profession[]
            {
                fortunate,
                popularHelper,
                lucky,
                unUnlucky,
                shootingStar,
                spiritChild
            };
        }
    }
}
