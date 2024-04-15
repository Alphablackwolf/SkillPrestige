using StardewValley;

namespace SkillPrestige.Bonuses.Factors
{
    /// <summary>
    /// Factor for determining if animal product quanitity should increase.
    /// </summary>
    public static class AnimalProduceFactor
    {
        public static decimal QuantityIncreaseChance { private get; set; }

        public static bool ShouldAnimalProductQuanityIncrease()
        {
            var randomizedValue = Game1.random.Next(1, 10);
            return QuantityIncreaseChance * 10 >= randomizedValue;
        }
    }
}