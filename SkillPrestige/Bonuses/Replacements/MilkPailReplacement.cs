using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Bonuses.Factors;
using StardewValley;

namespace SkillPrestige.Bonuses.Replacements
{
    /// <summary>
    /// A holder to contain replacement routines for the Stardew Valley MilkPail class.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global - used through reflection.
    public class MilkPailReplacement : Tool
    {
        // ReSharper disable once InconsistentNaming - needed for method replacement.
        private FarmAnimal animal;

        /// <summary>
        /// For some reason without this constructor the DoFunction code will cause execution exceptions at runtime.
        /// </summary>
        public MilkPailReplacement()
            : base("Milk Pail", -1, 6, 6, false)
        {
        }

        /// <summary>
        /// reqired by inheritance contract, not replaced so not important.
        /// </summary>
        /// <returns></returns>
        protected override string loadDisplayName()
        {
            return string.Empty;
        }

        /// <summary>
        /// reqired by inheritance contract, not replaced so not important.
        /// </summary>
        /// <returns></returns>
        protected override string loadDescription()
        {
            return string.Empty;
        }

        /// <summary>
        /// Pulled from the decompiled Stardew Valley code, *slightly* reworked for readability. 
        /// Added quantity adjustment.
        /// </summary>
        public override void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            base.DoFunction(location, x, y, power, who);
            who.Stamina -= 4f;
            currentParentTileIndex = 6;
            indexOfMenuItemView = 6;
            if (animal != null && animal.currentProduce > 0 && animal.age >= animal.ageWhenMature && animal.toolUsedForHarvest.Equals(name))
            {
                var farmer = who;
                var @object = new Object(Vector2.Zero, animal.currentProduce, null, false, true, false, false)
                    {
                        quality = animal.produceQuality
                    };
                if (farmer.addItemToInventoryBool(@object))
                {
                    if (AnimalProduceFactor.ShouldAnimalProductQuanityIncrease()) farmer.addItemToInventoryBool(@object.getOne());
                    Game1.playSound("coin");
                    animal.currentProduce = -1;
                    if (animal.showDifferentTextureWhenReadyForHarvest)
                        animal.sprite.Texture = Game1.content.Load<Texture2D>("Animals\\Sheared" + animal.type);
                    who.gainExperience(0, 5);
                }
            }
            animal = null;
            if (Game1.activeClickableMenu == null)
                who.CanMove = true;
            else
                who.Halt();
            who.usingTool = false;
            who.canReleaseTool = true;
        }
    }
}