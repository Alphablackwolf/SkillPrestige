using System;
using System.Collections.Generic;
using SkillPrestige.Bonuses.Factors;
using SkillPrestige.Bonuses.Handlers;
using SkillPrestige.Logging;
using SkillType = SkillPrestige.Skills.Types.SkillType;

namespace SkillPrestige.Bonuses.TypeRegistration
{
    // ReSharper disable once UnusedMember.Global - created through reflection.
    public sealed class FarmingBonusTypeRegistration : BonusTypeRegistration
    {
        public override void RegisterBonusTypes()
        {
            Logger.LogInformation("Registering Farming bonus types...");
            const int farmingToolProficiencyMaxLevel = 5;
            FarmingToolProficiency = new BonusType
            {
                Code = "Farming1",
                Name = "Tool Proficiency",
                MaxLevel = farmingToolProficiencyMaxLevel,
                EffectDescriptions = new List<string>
                {
                    "+4 Hoe Proficiency.",
                    "+4 Watering Can Proficiency."
                },
                SkillType = SkillType.Farming,
                ApplyEffect = x =>
                {
                    if (ToolProficiencyHandler.AddedToolProficencies.ContainsKey(ToolType.Hoe))
                    {
                        ToolProficiencyHandler.AddedToolProficencies[ToolType.Hoe] = x*4;
                    }
                    else
                    {
                        ToolProficiencyHandler.AddedToolProficencies.Add(ToolType.Hoe, x*4);
                    }
                    if (ToolProficiencyHandler.AddedToolProficencies.ContainsKey(ToolType.WateringCan))
                    {
                        ToolProficiencyHandler.AddedToolProficencies[ToolType.WateringCan] = x * 4;
                    }
                    else
                    {
                        ToolProficiencyHandler.AddedToolProficencies.Add(ToolType.WateringCan, x * 4);
                    }
                },
                GetLevelEffect = x => x <= farmingToolProficiencyMaxLevel ? $"+{x * 4} Hoe Proficiency{Environment.NewLine}+{x * 4} Watering Can Proficiency" : "MAX LEVEL."
            };
            const int betterCropsMaxLevel = 10;
            BetterCrops = new BonusType
            {
                Code = "Farming2",
                Name = "Better Crops",
                MaxLevel = betterCropsMaxLevel,
                EffectDescriptions = new List<string>
                {
                    "+10% chance of better quality crops."
                },
                SkillType = SkillType.Farming,
                ApplyEffect = x => CropQualityFactor.QualityImprovementChance = x/10m,
                GetLevelEffect = x => x <= betterCropsMaxLevel ? $"+{x * 10}% chance of better quality crops." : "MAX LEVEL."
            };
            const int efficientAnimalsMaxLevel = 5;
            EfficientAnimals = new BonusType
            {
                Code = "Farming3",
                Name = "Efficient Animals",
                MaxLevel = efficientAnimalsMaxLevel,
                EffectDescriptions = new List<string>
                {
                    "+20% chance of receiving double animal products.",
                    "(This does not affect truffles, which count as foraged items)"
                },
                SkillType = SkillType.Farming,
                ApplyEffect = x =>AnimalProduceFactor.QuantityIncreaseChance = x/5m,
                GetLevelEffect = x => x <= efficientAnimalsMaxLevel ? $"+{x * 20}% chance of receiving double animal products." : "MAX LEVEL."
            };
            const int regrowthOpportunityMaxLevel = 5;
            RegrowthOpportunity = new BonusType
            {
                Code = "Farming4",
                Name = "Regrowth Opportunity",
                MaxLevel = regrowthOpportunityMaxLevel,
                EffectDescriptions = new List<string>
                {
                    "+5% chance of receiving seeds with crops.",
                    "At max level, gives a 1/3 chance of receiving seeds from dead plants."
                },
                SkillType = SkillType.Farming,
                ApplyEffect = x =>
                {
                    CropRegrowthFactor.RegrowthChance = x / 20m;
                    if (x == MaxLevel)
                    {
                        CropRegrowthFactor.DeadRegrowthChance = 1 / 3m;
                    }
                },
                GetLevelEffect = x => x <= regrowthOpportunityMaxLevel ? $"+{x * 5}% chance of receiving seeds with crops.{(x == regrowthOpportunityMaxLevel ? $"{Environment.NewLine}1/3 chance of receiving seeds from dead plants" : string.Empty)}" : "MAX LEVEL."
            };
            Logger.LogInformation("Farming bonus types registered.");
        }
    }
}
