using StardewValley;

namespace SkillPrestige.Framework
{
    /// <summary>Provides an access point to determine crop quality adjustments.</summary>
    internal static class CropQualityFactor
    {

        public static decimal QualityImprovementChance { private get; set; }

        public static int GetCropQualityIncrease()
        {
            int randomizedValue = Game1.random.Next(1, 10);
            return QualityImprovementChance * 10 >= randomizedValue ? 1 : 0;
        }
    }
}
