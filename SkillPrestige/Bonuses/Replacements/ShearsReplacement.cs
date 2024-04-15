using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkillPrestige.Bonuses.Factors;
using SkillPrestige.Logging;
using StardewValley;
using StardewValley.Tools;

namespace SkillPrestige.Bonuses.Replacements
{
    /// <summary>
    /// A holder to contain replacement routines for the Stardew Valley Shears class.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global - is used through reflection.
    public class ShearsReplacement : Tool
    {
        // ReSharper disable once InconsistentNaming - needed for method replacement.
        private FarmAnimal animal;

        /// <summary>
        /// For some reason without this constructor the DoFunction code will cause execution exceptions at runtime.
        /// </summary>
        public ShearsReplacement()
            : base("Shears", -1, 7, 7, false)
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
        /// Replaces the existing DoFunction in Shears, adding a quantity increase.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="power"></param>
        /// <param name="who"></param>
        public new void DoFunction(GameLocation location, int x, int y, int power, StardewValley.Farmer who)
        {
            Logger.LogInformation("using shears...");
            base.DoFunction(location, x, y, power, who);
            who.Stamina -= 4f;
            Shears.playSnip(who);
            currentParentTileIndex = 7;
            indexOfMenuItemView = 7;
            if (animal != null && animal.currentProduce > 0 && animal.age >= animal.ageWhenMature && animal.toolUsedForHarvest.Equals(name))
            {
                Logger.LogInformation("shearing...");
                var farmer = who;
                var @object = new StardewValley.Object(Vector2.Zero, animal.currentProduce, null, false, true, false, false)
                {
                    quality = animal.produceQuality
                };
                if (farmer.addItemToInventoryBool(@object))
                {
                    if (AnimalProduceFactor.ShouldAnimalProductQuanityIncrease())
                    {
                        Logger.LogInformation("Adding item through shears.");
                        farmer.addItemToInventoryBool(@object.getOne());
                    }
                    animal.currentProduce = -1;
                    Game1.playSound("coin");
                    animal.friendshipTowardFarmer = Math.Min(1000, animal.friendshipTowardFarmer + 5);
                    if (animal.showDifferentTextureWhenReadyForHarvest)
                        animal.sprite.Texture = Game1.content.Load<Texture2D>("Animals\\Sheared" + animal.type);
                    who.gainExperience(0, 5);
                }
            }
            else
            {
                var dialogue = "";
                if (animal != null && !animal.toolUsedForHarvest.Equals(name))
                    dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14245", animal.displayName as object);
                if (animal != null && animal.isBaby() && animal.toolUsedForHarvest.Equals(name))
                    dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14246", animal.displayName as object);
                if (animal != null && animal.age >= animal.ageWhenMature && animal.toolUsedForHarvest.Equals(name))
                    dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14247", animal.displayName as object);
                if (dialogue.Length > 0)
                    Game1.drawObjectDialogue(dialogue);
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
