using SkillPrestige.Logging;
using StardewValley;

namespace SkillPrestige.Bonuses.Factors
{
    /// <summary>
    /// Provides an access point to determine crop quality adjustments.
    /// </summary>
    public static class CropQualityFactor
    {

        public static decimal QualityImprovementChance { private get; set; }

        public static int GetCropQualityIncrease()
        {
            var randomizedValue = Game1.random.Next(1, 10);
            var qualityIncreaseAmount = QualityImprovementChance * 10 >= randomizedValue ? 1 : 0;
            Logger.LogInformation($"Crop quality increase: {qualityIncreaseAmount}, randomizedValue = {randomizedValue}, QualityImprovementChance: {QualityImprovementChance}");
            return qualityIncreaseAmount;
        }

    }
}