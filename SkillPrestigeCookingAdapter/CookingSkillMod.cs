using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SkillPrestige;
using SkillPrestige.Mods;
using SkillPrestige.Professions;
using SkillPrestige.SkillTypes;
using StardewValley;

namespace SkillPrestigeCookingAdapter
{
    public class CookingSkillMod : SkillMod
    {
        public override string DisplayName => "Cooking Skill";
        protected override string Namespace => "CookingSkill";
        protected override string ClassName => "CookingSkillMod";
        public override IEnumerable<Skill> AdditionalSkills => IsFound ?
        new List<Skill>
            {
                new Skill
                {
                    Type = CookingSkillType,
                    SkillScreenPosition = Type.GetType("LuckSkill.LuckSkillMod, LuckSkill") == null ? 6 : 7, //variable due to potential conflict with order due to luck skill mod.
                    SourceRectangleForSkillIcon = new Rectangle(0, 0, 16, 16),
                    SkillIconTexture = SkillPrestigeCookingAdapterMod.GetCookingSkillTexture(),
                    Professions = GetCookingProfessions(),
                    SetSkillLevel = x => {}, //No set necessary, as the level is not stored independently from the experience.
                    GetSkillLevel = GetCookingLevel
                }
            } : null;

        public override IEnumerable<Prestige> AdditonalPrestiges => IsFound ? new List<Prestige>
        {
            new Prestige
            {
                SkillType = CookingSkillType
            }
        } : null;

        private static SkillType CookingSkillType => _cookingSkillType ?? (_cookingSkillType = new SkillType("Cooking", 6));

        private static SkillType _cookingSkillType;

        private static int GetCookingLevel()
        {
            FixExpLength();
            for (var index = ExpNeededForLevel.Length - 1; index >= 0; --index)
            {
                if (Game1.player.experiencePoints[6] >= ExpNeededForLevel[index])
                    return index + 1;
            }
            return 0;
        }

        private static void FixExpLength()
        {
            if (Game1.player.experiencePoints.Length >= 7)
                return;
            var newExperienceArray = new int[7];
            for (var index = 0; index < 6; ++index)
                newExperienceArray[index] = Game1.player.experiencePoints[index];
            Game1.player.experiencePoints = newExperienceArray;
        }

        private static readonly int[] ExpNeededForLevel =
        {
            100,
            380,
            770,
            1300,
            2150,
            3300,
            4800,
            6900,
            10000,
            15000
        };

        private static List<Profession> GetCookingProfessions()
        {
            var gourmet = new TierOneProfession
            {
                Id = 50,
                DisplayName = "Gourmet",
                EffectText = new[] {"+20% sell price"}
            };
            var satisfying = new TierOneProfession
            {
                Id = 51,
                DisplayName = "Satisfying",
                EffectText = new[] {"+25% buff duration once eaten"}
            };
            var efficient = new TierTwoProfession
            {
                Id = 52,
                DisplayName = "Efficient",
                EffectText = new[] {"15% chance to not consume ingredients"},
                TierOneProfession = gourmet
            };
            var professionalChef = new TierTwoProfession
            {
                Id = 53,
                DisplayName = "Prof. Chef",
                EffectText = new[] { "Home-cooked meals are always at least silver" },
                TierOneProfession = gourmet
            };
            var intenseFlavors = new TierTwoProfession
            {
                Id = 54,
                DisplayName = "Intense Flavors",
                EffectText = new[]
                {
                    "Food buffs are one level stronger",
                    "(+20% for max energy or magnetism)"
                },
                TierOneProfession = satisfying
            };
            var secretSpices = new TierTwoProfession
            {
                Id = 55,
                DisplayName = "Secret Spices",
                EffectText = new[]{"Provides a few random buffs when eating unbuffed food."},
                TierOneProfession = satisfying
            };
            gourmet.TierTwoProfessions = new List<TierTwoProfession>
            {
                efficient,
                professionalChef
            };
            satisfying.TierTwoProfessions = new List<TierTwoProfession>
            {
                intenseFlavors,
                secretSpices
            };
            return new List<Profession>
            {
                gourmet,
                satisfying,
                efficient,
                professionalChef,
                intenseFlavors,
                secretSpices
            };
        }

    }
}
