using StardewValley;

namespace SkillPrestige.Framework
{
    /// <summary>Provides an access point to determine crop 'regrowth' adjustments (seeds given at harvest time).</summary>
    internal static class CropRegrowthFactor
    {
        public static decimal RegrowthChance { get; set; }
        public static decimal DeadRegrowthChance { private get; set; }

        public static bool GetCropRegrowthSuccess()
        {
            var randomizedValue = Game1.random.Next(1, 10);
            return RegrowthChance * 10 >= randomizedValue;
        }

        public static bool GetDeadCropRegrowthSuccess()
        {
            var randomizedValue = Game1.random.Next(1, 100);
            return DeadRegrowthChance * 100 >= randomizedValue;
        }
    }
}
